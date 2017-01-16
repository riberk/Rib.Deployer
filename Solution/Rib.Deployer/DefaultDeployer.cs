namespace Rib.Deployer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Common.Logging;
    using JetBrains.Annotations;

    public class DefaultDeployer : IDeployer
    {
        [NotNull] private readonly ILog _logger;
        [NotNull, ItemNotNull] private readonly IReadOnlyCollection<IDeployStep> _steps;

        public DefaultDeployer([ItemNotNull] [NotNull] params IDeployStep[] steps) : this(null, steps)
        {
        }

        public DefaultDeployer(ILog logger, [ItemNotNull] [NotNull] params IDeployStep[] steps)
        {
            if (steps == null) throw new ArgumentNullException(nameof(steps));
            if (steps.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(steps));
            _steps = steps;
            _logger = logger ?? DeployerContext.LoggerFactory.Create(typeof(DefaultDeployer));
        }

        public void Deploy()
        {
            var watch = new Stopwatch();
            var executedSteps = new Stack<IDeployStep>();

            foreach (var deployStep in _steps)
            {
                try
                {
                    _logger.Info($"Start {deployStep.Name} step with type {deployStep.GetType()}");
                    watch.Start();
                    deployStep.Apply();
                    executedSteps.Push(deployStep);
                    watch.Stop();
                    _logger.Info($"End {deployStep.Name} step");
                }
                catch (Exception e)
                {
                    watch.Stop();
                    _logger.Error($"Exception on step {deployStep.Name}", e);

                    try
                    {
                        Rollback(executedSteps);
                    }
                    catch (Exception revokeException)
                    {
                        _logger.Error("Exception on revoking", revokeException);
                        throw;
                    }
                    return;
                }
                finally
                {
                    _logger.Info($"Step {deployStep.Name} ended on {watch.Elapsed}");
                    watch.Reset();
                }
            }
            foreach (var deployStep in _steps)
            {
                try
                {
                    _logger.Info($"Closing {deployStep.Name} step");
                    var disposable = deployStep as IDisposable;
                    disposable?.Dispose();
                }
                catch (Exception e)
                {
                    _logger.Error($"Error on closing step {deployStep.Name}", e);
                }
            }
            _logger.Info("Deploy finished");
        }

        private void Rollback([NotNull, ItemNotNull] IEnumerable<IDeployStep> executedSteps)
        {
            var watch = new Stopwatch();
            foreach (var executedStep in executedSteps)
            {
                _logger.Info($"Rolback {executedStep.Name} with type {executedStep.GetType()}");
                watch.Start();
                try
                {
                    executedStep.Rollback();
                    _logger.Info($"Rolback {executedStep.Name} finished");
                    watch.Stop();
                }
                catch (Exception e)
                {
                    watch.Stop();
                    _logger.Error($"Exception on rollback step {executedStep.Name}", e);
                    throw;
                }
                finally
                {
                    _logger.Info($"Step {executedStep.Name} rollback on {watch.Elapsed}");
                    watch.Reset();
                }
            }
        }
    }
}
namespace Rib.Deployer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Common.Logging;
    using JetBrains.Annotations;

    public class DefaultDeployer : IDeployer
    {
        [NotNull] private static readonly ILog Logger = DeployerContext.LoggerFactory.Create(typeof(DefaultDeployer));
        [NotNull, ItemNotNull] private readonly IReadOnlyCollection<IDeployStep> _steps;

        public DefaultDeployer([ItemNotNull] [NotNull] params IDeployStep[] steps)
        {
            if (steps == null) throw new ArgumentNullException(nameof(steps));
            if (steps.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(steps));
            _steps = steps;
        }

        public void Deploy()
        {
            var watch = new Stopwatch();
            var executedSteps = new Stack<IDeployStep>();

            foreach (var deployStep in _steps)
            {
                try
                {
                    Logger.Info($"Start {deployStep.Name} step with type {deployStep.GetType()}");
                    watch.Start();
                    deployStep.Apply();
                    executedSteps.Push(deployStep);
                    watch.Stop();
                    Logger.Info($"End {deployStep.Name} step");
                }
                catch (Exception e)
                {
                    watch.Stop();
                    Logger.Error($"Exception on step {deployStep.Name}", e);

                    try
                    {
                        Rollback(executedSteps);
                    }
                    catch (Exception revokeException)
                    {
                        Logger.Error("Exception on revoking", revokeException);
                        throw;
                    }
                    return;
                }
                finally
                {
                    Logger.Info($"Step {deployStep.Name} ended on {watch.Elapsed}");
                    watch.Reset();
                }
            }
            foreach (var deployStep in _steps)
            {
                try
                {
                    Logger.Info($"Closing {deployStep.Name} step");
                    var disposable = deployStep as IDisposable;
                    disposable?.Dispose();
                }
                catch (Exception e)
                {
                    Logger.Error($"Error on closing step {deployStep.Name}", e);
                }
            }
            Logger.Info("Deploy finished");
        }

        private static void Rollback([NotNull, ItemNotNull] IEnumerable<IDeployStep> executedSteps)
        {
            var watch = new Stopwatch();
            foreach (var executedStep in executedSteps)
            {
                Logger.Info($"Rolback {executedStep.Name} with type {executedStep.GetType()}");
                watch.Start();
                try
                {
                    executedStep.Rollback();
                    Logger.Info($"Rolback {executedStep.Name} finished");
                    watch.Stop();
                }
                catch (Exception e)
                {
                    watch.Stop();
                    Logger.Error($"Exception on rollback step {executedStep.Name}", e);
                    throw;
                }
                finally
                {
                    Logger.Info($"Step {executedStep.Name} rollback on {watch.Elapsed}");
                    watch.Reset();
                }
            }
        }
    }
}
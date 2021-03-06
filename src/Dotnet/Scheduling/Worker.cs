﻿using Dotnet.Dependency;
using Dotnet.Logging;
using System;
using System.Threading;

namespace Dotnet.Scheduling
{
    /// <summary>Represent a background worker that will repeatedly execute a specific method.
    /// </summary>
    public class Worker
    {
        private readonly object _lockObject = new object();
        private readonly ILogger _logger;
        private readonly string _actionName;
        private readonly Action _action;
        private Status _status;

        /// <summary>Returns the action name of the current worker.
        /// </summary>
        public string ActionName => _actionName;

        /// <summary>Initialize a new worker with the specified action.
        /// </summary>
        public Worker(string actionName, Action action)
        {
            _actionName = actionName;
            _action = action;
            _status = Status.Initial;
            _logger = IocManager.GetContainer().Resolve<ILoggerFactory>().Create(DotnetConsts.LoggerName);
        }

        /// <summary>Start the worker if it is not running.
        /// </summary>
        public Worker Start()
        {
            lock (_lockObject)
            {
                if (_status == Status.Running) return this;

                _status = Status.Running;
                new Thread(Loop)
                {
                    Name = $"{_actionName}.Worker",
                    IsBackground = true
                }.Start(this);

                return this;
            }
        }
        /// <summary>Request to stop the worker.
        /// </summary>
        public Worker Stop()
        {
            lock (_lockObject)
            {
                if (_status == Status.StopRequested) return this;

                _status = Status.StopRequested;

                return this;
            }
        }

        private void Loop(object data)
        {
            var worker = (Worker)data;

            while (worker._status == Status.Running)
            {
                try
                {
                    _action();
                }
                catch (ThreadAbortException)
                {
                    _logger.InfoFormat("Worker thread caught ThreadAbortException, try to resetting, actionName:{0}", _actionName);
                    Thread.ResetAbort();
                    _logger.InfoFormat("Worker thread ThreadAbortException resetted, actionName:{0}", _actionName);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Worker thread has exception, actionName:{_actionName},error:{ex.Message}");
                }
            }
        }

        enum Status
        {
            Initial,
            Running,
            StopRequested
        }
    }
}

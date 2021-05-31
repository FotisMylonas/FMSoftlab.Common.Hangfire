using Hangfire;
using Hangfire.Client;
using Hangfire.States;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FMSoftlab.Common.Hangfire
{
    public class FMSoftlabHangfireQueueManager
    {
        private string QueueName { get; }


        private const string QueuePropertyName = "StashedQueueName";
        /*public class Hangfire_QueueNameSetter : IClientFilter
        {
            public void OnCreating(CreatingContext filterContext)
            {
                EnqueuedState enqueuedState = filterContext.InitialState as EnqueuedState;
                if (enqueuedState!=null
                    //&& string.IsNullOrWhiteSpace(enqueuedState?.Queue))
                {
                    filterContext.SetJobParameter(QueuePropertyName, QueueName);
                }
            }
            public void OnCreated(CreatedContext filterContext) { }
        }*/

        public class Hangfire_QueueNameFixer : IElectStateFilter
        {
            public void OnStateElection(ElectStateContext context)
            {
                /*var queueName = context.GetJobParameter<string>(QueuePropertyName);

                if (context.CandidateState.Name == "Enqueued" &&
                    !string.IsNullOrWhiteSpace(queueName))
                {
                    context.CandidateState = new EnqueuedState(queueName);
                }*/

                var enqueuedState = context.CandidateState as EnqueuedState;
                if (enqueuedState != null)
                {
                    var qn = context.GetJobParameter<string>(QueuePropertyName);
                    if (!String.IsNullOrWhiteSpace(qn))
                    {
                        enqueuedState.Queue = qn;
                    }
                    else
                    {
                        context.SetJobParameter(QueuePropertyName, enqueuedState.Queue);
                    }
                }
            }
        }

        public void InitHangifre()
        {
            //GlobalJobFilters.Filters.Add(new Hangfire_QueueNameSetter());
            GlobalJobFilters.Filters.Add(new Hangfire_QueueNameFixer());
            //HalloWorld();
        }

        private void EnqueueJob(Expression<Action> methodCall, string queueName)
        {
            var client = new BackgroundJobClient();
            client.Create(methodCall, new EnqueuedState(queueName));
        }

        private void EnqueueJob(Expression<Func<Task>> methodCall, string queueName)
        {
            var client = new BackgroundJobClient();
            client.Create(methodCall, new EnqueuedState(queueName));
        }

        private void EnqueueJob<T>(Expression<Action<T>> methodCall, string queueName)
        {
            var client = new BackgroundJobClient();
            client.Create(methodCall, new EnqueuedState(queueName));
        }

        private void EnqueueJob<T>(Expression<Func<T, Task>> methodCall, string queueName)
        {
            var client = new BackgroundJobClient();
            client.Create(methodCall, new EnqueuedState(queueName));
        }
        public FMSoftlabHangfireQueueManager(string queueName)
        {
            QueueName = queueName;
            InitHangifre();
        }
        public void HalloWorld()
        {
            EnqueueJob(() => Console.WriteLine("Hello world from Hangfire!"), QueueName);
        }

        public void EnqueueJob(Expression<Action> methodCall)
        {
            EnqueueJob(methodCall, QueueName);
        }

        public void EnqueueJob(Expression<Func<Task>> methodCall)
        {
            EnqueueJob(methodCall, QueueName);
        }

        public void EnqueueJob<T>(Expression<Action<T>> methodCall)
        {
            EnqueueJob(methodCall, QueueName);
        }

        public void EnqueueJob<T>(Expression<Func<T, Task>> methodCall)
        {
            EnqueueJob(methodCall, QueueName);
        }
    }
}

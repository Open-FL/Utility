using System;

namespace Utility.ObjectPipeline
{
    public static class PipelineExtensions
    {

        public static Func<object, object> GetDelegate(this PipelineStage stage)
        {
            return stage.Process;
        }

        public static Func<In, Out> GetDelegate<In, Out>(this PipelineStage<In, Out> stage)
        {
            return stage.Process;
        }

        public static DelegatePipelineStage<object, object> GetPipelineStage(Func<object, object> func)
        {
            return new DelegatePipelineStage<object, object>(input => func(input));
        }

        public static DelegatePipelineStage<In, Out> GetPipelineStage<In, Out>(Func<In, Out> func)
        {
            return new DelegatePipelineStage<In, Out>(input => func(input));
        }

    }
}
using System;

namespace Utility.ObjectPipeline
{
    public class DelegatePipelineStage<TIn, TOut> : PipelineStage<TIn, TOut>
    {

        public delegate TOut ProcessDel(TIn input);

        private readonly ProcessDel stageDel;

        public DelegatePipelineStage(ProcessDel stageDel)
        {
            this.stageDel =
                stageDel ?? throw new ArgumentNullException("stageDel", "The Stage Delegate can not be null.");
        }

        public override TOut Process(TIn input)
        {
            return stageDel(input);
        }

    }
}
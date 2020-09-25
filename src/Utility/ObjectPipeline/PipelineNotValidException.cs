using Utility.Exceptions;

namespace Utility.ObjectPipeline
{
    public class PipelineNotValidException : Byt3Exception
    {

        public PipelineNotValidException(PipelineStage pipeline, string message) : base(message)
        {
            Pipeline = pipeline;
        }

        public PipelineStage Pipeline { get; }

    }
}
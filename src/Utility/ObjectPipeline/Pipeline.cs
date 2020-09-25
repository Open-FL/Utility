using System;

namespace Utility.ObjectPipeline
{
    public class Pipeline : Pipeline<PipelineStage>
    {

        public Pipeline(Type inType, Type outType, params PipelineStage[] stages) : base(inType, outType)
        {
            for (int i = 0; i < stages.Length; i++)
            {
                AddSubStage(stages[i]);
            }
        }


        public override object Process(object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Argument is not allowed to be null.");
            }

            if (!Verified && !Verify())
            {
                throw new PipelineNotValidException(this, "Can not use a Pipeline that is incomplete.");
            }

            object currentIn = input;
            foreach (PipelineStage internalPipelineStage in Stages)
            {
                currentIn = internalPipelineStage.Process(currentIn);
            }

            return currentIn;
        }

    }

    public class Pipeline<I, O> : Pipeline<PipelineStage, I, O>
    {

    }

    public class Pipeline<T, I, O> : Pipeline<T>
        where T : PipelineStage
    {

        public Pipeline() : base(typeof(I), typeof(O))
        {
        }

        public O Process(I input)
        {
            return (O) Process((object) input);
        }

    }

    public class Pipeline<T> : PipelineStage<T>
        where T : PipelineStage
    {

        public Pipeline(Type inType, Type outType, params T[] stages) : base(inType, outType)
        {
            for (int i = 0; i < stages.Length; i++)
            {
                AddSubStage(stages[i]);
            }
        }

        public override object Process(object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "Argument is not allowed to be null.");
            }

            if (!Verified && !Verify())
            {
                throw new PipelineNotValidException(this, "Can not use a Pipline that is incomplete.");
            }

            object currentIn = input;
            foreach (T internalPipelineStage in Stages)
            {
                currentIn = internalPipelineStage.Process(currentIn);
            }

            return currentIn;
        }

    }
}
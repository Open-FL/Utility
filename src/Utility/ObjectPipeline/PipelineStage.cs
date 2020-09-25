using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.ObjectPipeline
{
    public abstract class PipelineStage
    {

        protected PipelineStage(Type inType, Type outType)
        {
            InType = inType;
            OutType = outType;
        }

        public Type InType { get; protected set; }

        public Type OutType { get; protected set; }

        public abstract object Process(object input);

    }

    public abstract class PipelineStage<StageBase> : PipelineStage
        where StageBase : PipelineStage
    {

        protected List<StageBase> Stages = new List<StageBase>();

        protected PipelineStage(Type inType, Type outType) : base(inType, outType)
        {
        }


        protected bool Verified { get; private set; }

        public void RemoveSubStage(StageBase stage)
        {
            if (stage == null)
            {
                throw new ArgumentNullException("stage", "Argument is not allowed to be null.");
            }

            Stages.Remove(stage);
            Verified = false;
        }

        public void RemoveSubStageAt(int index)
        {
            if (index > 0 && index < Stages.Count)
            {
                Stages.RemoveAt(index);
            }
        }

        public void InsertAtFirstValidIndex(StageBase stage)
        {
            Type targetOutput = stage.InType;
            Type targetInput = stage.OutType;
            for (int i = 0; i < Stages.Count; i++)
            {
                if (targetOutput.IsAssignableFrom(Stages[i].OutType))
                {
                    if (i == Stages.Count - 1)
                    {
                        AddSubStage(stage); //If Empty or Last item we want to make sure that the 
                        return;
                    }

                    if (Stages[i + 1].InType.IsAssignableFrom(targetInput))
                    {
                        Stages.Insert(i + 1, stage);
                        return;
                    }
                }
            }

            throw new PipelineNotValidException(
                                                this,
                                                $"Can not Add stage In:{stage.InType} Out: {stage.OutType} because there is no valid Index for this Stage in the pipeline."
                                               );
        }

        public void AddSubStage(StageBase stage)
        {
            if (stage == null)
            {
                throw new ArgumentNullException("stage", "Argument is not allowed to be null.");
            }

            Verified = false;
            if (Stages.Count == 0)
            {
                if (!stage.InType.IsAssignableFrom(InType))
                {
                    throw new PipelineNotValidException(
                                                        this,
                                                        $"Can not Add stage with in type {stage.InType} as first element in the Pipeline. it has to be the same type as the pipeline in type({InType})"
                                                       );
                }

                Stages.Add(stage);
            }
            else
            {
                PipelineStage last = Stages.Last();
                if (!stage.InType.IsAssignableFrom(last.OutType))
                {
                    throw new PipelineNotValidException(
                                                        this,
                                                        $"Can not Add stage with in type {stage.InType} in the pipeline. it has to be the same type as the previous pipeline out type({last.OutType})"
                                                       );
                }

                Stages.Add(stage);
            }
        }

        public bool Verify()
        {
            if (Stages.Count == 0)
            {
                Verified = true;
                return true;
            }


            bool ret = true;

            for (int i = 0; i < Stages.Count; i++)
            {
                if (Stages[i] is Pipeline)
                {
                    ret &= (Stages[i] as Pipeline).Verify();
                }
            }

            Verified = ret && Stages.Last().OutType == OutType;
            return Verified;
        }

    }


    public abstract class PipelineStage<I, O> : PipelineStage
    {

        protected PipelineStage() : base(typeof(I), typeof(O))
        {
        }

        public override object Process(object input)
        {
            return Process((I) input);
        }


        public abstract O Process(I input);

    }
}
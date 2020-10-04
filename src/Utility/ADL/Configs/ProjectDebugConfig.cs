using System;

namespace Utility.ADL.Configs
{
    public class ProjectDebugConfig : IProjectDebugConfig
    {

        public static event Action<ProjectDebugConfig> OnConfigCreate = null;

        public ProjectDebugConfig(
            string projectName, int acceptMask, int minSeverity,
            PrefixLookupSettings lookupSettings)
        {
            ProjectName = projectName;
            AcceptMask = acceptMask;
            MinSeverity = minSeverity;
            PrefixLookupSettings = lookupSettings;
            OnConfigCreate?.Invoke(this);
        }

        public string ProjectName { get; set; }

        public int AcceptMask { get; set; }

        public int MinSeverity { get; set; }

        public PrefixLookupSettings PrefixLookupSettings { get; set; }

        public virtual string GetProjectName()
        {
            return ProjectName;
        }

        public virtual int GetMinSeverity()
        {
            return MinSeverity;
        }

        public virtual int GetAcceptMask()
        {
            return AcceptMask;
        }

        public virtual PrefixLookupSettings GetPrefixLookupSettings()
        {
            return PrefixLookupSettings;
        }


        public virtual void SetProjectName(string projectName)
        {
            ProjectName = projectName;
        }

        public virtual void SetMinSeverity(int severity)
        {
            MinSeverity = severity;
        }

        public virtual void SetAcceptMask(int mask)
        {
            AcceptMask = mask;
        }

        public virtual void SetPrefixLookupSettings(PrefixLookupSettings settings)
        {
            PrefixLookupSettings = settings;
        }

        public override string ToString()
        {
            return ProjectName;
        }

    }

    public class ProjectDebugConfig<MaskType, SeverityType> : IProjectDebugConfig
        where MaskType : Enum
        where SeverityType : Enum
    {

        public ProjectDebugConfig(
            string projectName, MaskType acceptMask, SeverityType minSeverity,
            PrefixLookupSettings lookupSettings)
        {
            ProjectName = projectName;
            AcceptMask = acceptMask;
            MinSeverity = minSeverity;
            PrefixLookupSettings = lookupSettings;
        }

        public string ProjectName { get; set; }

        public MaskType AcceptMask { get; set; }

        public SeverityType MinSeverity { get; set; }

        public PrefixLookupSettings PrefixLookupSettings { get; set; }

        public virtual string GetProjectName()
        {
            return ProjectName;
        }

        public virtual int GetMinSeverity()
        {
            return Convert.ToInt32(MinSeverity);
        }

        public virtual int GetAcceptMask()
        {
            return Convert.ToInt32(AcceptMask);
        }

        public virtual PrefixLookupSettings GetPrefixLookupSettings()
        {
            return PrefixLookupSettings;
        }

        public virtual void SetProjectName(string projectName)
        {
            ProjectName = projectName;
        }

        public virtual void SetMinSeverity(int severity)
        {
            MinSeverity = (SeverityType) Enum.ToObject(typeof(SeverityType), severity);
        }

        public virtual void SetAcceptMask(int mask)
        {
            AcceptMask = (MaskType) Enum.ToObject(typeof(MaskType), mask);
        }

        public virtual void SetPrefixLookupSettings(PrefixLookupSettings settings)
        {
            PrefixLookupSettings = settings;
        }

        public override string ToString()
        {
            return ProjectName;
        }

    }
}
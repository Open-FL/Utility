using System.Collections.Generic;

namespace Utility.CommandRunner.BuiltInCommands.SetSettings
{
    public struct FieldInformations
    {

        public Dictionary<string, FieldInformation> Fields;

        public FieldInformations(Dictionary<string, FieldInformation> fields)
        {
            Fields = fields;
        }

    }
}
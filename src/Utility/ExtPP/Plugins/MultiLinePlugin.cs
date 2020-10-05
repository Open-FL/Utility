﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Base.settings;

namespace Utility.ExtPP.Plugins
{
    public class MultiLinePlugin : AbstractFullScriptPlugin
    {

        public override ProcessStage ProcessStages =>
            Stage.ToLower(CultureInfo.InvariantCulture) == "onload"
                ? ProcessStage.OnLoadStage
                : ProcessStage.OnMain;

        public string Stage { get; set; } = "onload";

        public string MultiLineKeyword { get; set; } = "__";

        public override string[] Prefix => new[] { "mlp", "MultiLine" };


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
                                                          {
                                                              new CommandInfo(
                                                                              "set-stage",
                                                                              "ss",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       MultiLinePlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Stage)
                                                                                  ),
                                                                              "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-mlkeyword",
                                                                              "mlk",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       MultiLinePlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       MultiLineKeyword
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to detect when to lines should be merged. The line containing the keyword will be merges with the next line in the file"
                                                                             )
                                                          };


        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);
        }

        public override bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            List<string> source = file.GetSource().ToList();
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (i < source.Count - 1 && source[i].TrimEnd().EndsWith(MultiLineKeyword))
                {
                    string newstr = source[i].Substring(0, source[i].Length - MultiLineKeyword.Length) + source[i + 1];
                    source.RemoveAt(i + 1);
                    source[i] = newstr;
                }
            }

            file.SetSource(source.ToArray());
            return true;
        }

    }
}
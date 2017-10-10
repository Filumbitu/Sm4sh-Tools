using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sm4shCommand
{
    public class EventConfig
    {
        public EventConfig()
        {
            Overrides = new Dictionary<uint, acmd_cmd_def>();
        }
        public EventConfig(string filepath): this()
        {
            ReadFromFile(filepath);
        }

        public Dictionary<uint, acmd_cmd_def> Overrides { get; set; }

        public void AddOverride(uint opcode, acmd_cmd_def definition)
        {
            if (Overrides.ContainsKey(opcode))
            {
                Overrides[opcode] = definition;
            }
            else
            {
                Overrides.Add(opcode, definition);
            }
        }
        public void RemoveOverride(uint opcode)
        {
            Overrides.Remove(opcode);
        }
        public bool IsOverridden(uint opcode)
        {
            return Overrides.ContainsKey(opcode);
        }

        public void SaveToFile(string filepath)
        {
            using (StreamWriter writer = File.CreateText(filepath))
            {
                foreach (KeyValuePair<uint, acmd_cmd_def> pair in Overrides)
                {
                    uint opcode = pair.Key;
                    acmd_cmd_def definition = pair.Value;

                    writer.WriteLine(opcode.ToString("X8"));
                    writer.WriteLine(definition.CommandName);
                    writer.WriteLine(string.Join(",", definition.ParamSpecifiers));
                    writer.WriteLine(string.Join(",", definition.ParameterNames));
                    writer.WriteLine(definition.Description);
                    writer.Write(Environment.NewLine);
                }
            }
        }
        public void ReadFromFile(string filepath)
        {
            Overrides.Clear();
            using (StreamReader reader = File.OpenText(filepath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                        continue;

                    var def = new acmd_cmd_def();

                    def.Opcode = uint.Parse(line, System.Globalization.NumberStyles.HexNumber);
                    def.CommandName = reader.ReadLine().Trim();
                    def.ParamSpecifiers = reader.ReadLine().Trim().Split(',').Where(x => x != "NONE").Select(x => uint.Parse(x)).Cast<acmd_param_type>().ToArray();
                    def.ParameterNames = reader.ReadLine().Trim().Split(',').Where(x => x != "NONE").ToArray();
                    def.Description = reader.ReadLine().Trim();

                    Overrides.Add(def.Opcode, def);
                }
            }
        }
        public void ApplyOverrides()
        {
            foreach (var def in Overrides.Values)
            {
                SALT.Moveset.AnimCMD.ACMD_INFO.SetCMDInfo(def.Opcode,
                                                          def.ParamSpecifiers.Length + 1,
                                                          def.CommandName,
                                                          def.ParamSpecifiers.Cast<int>().ToArray(),
                                                          def.ParameterNames);
            }
        }
    }

    public class acmd_cmd_def
    {
        public uint Opcode { get; set; }
        public string CommandName { get; set; }
        public acmd_param_type[] ParamSpecifiers { get; set; }
        public string[] ParameterNames { get; set; }
        public string Description { get; set; }
    }
    public enum acmd_param_type
    {
        Integer = 0,
        Float = 1,
        Decimal = 2
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Plot.Tools.PlotEditor
{
    public interface IJsonSwitch
    {
        JsonData jsonData { get; set; }
        void FromJson(JsonData jsonData);
        JsonData ToJson();
    }
}

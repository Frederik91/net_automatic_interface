﻿using AutomaticInterfaceAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNuget
{
    [GenerateAutomaticInterface]
    public class Test: ITest
    {
        public string GetString() { return "works"; }
    }
}

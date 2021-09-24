﻿using AutomaticInterface;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutomaticInterfaceAttribute;
using VerifyCS = Tests.CSharpSourceGeneratorVerifier<AutomaticInterface.AutomaticInterfaceGenerator>;

namespace Tests
{
    public class GeneratorTests
    {
        ImmutableArray<string> references = AppDomain.CurrentDomain.GetAssemblies()
                       .Where(assembly => !assembly.IsDynamic)
                       .Select(assembly => assembly.Location)
            .ToImmutableArray();

        private async Task RunTestAsync(string code, string expectedResult)
        {
            var tester = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { code },
                    GeneratedSources =
                    {
                        (typeof(AutomaticInterfaceGenerator), "IDemoClass.cs", SourceText.From(expectedResult, Encoding.UTF8, SourceHashAlgorithm.Sha1)),
                    },
                },
            };

            tester.ReferenceAssemblies = ReferenceAssemblies.Net.Net50;
            tester.ReferenceAssemblies.AddAssemblies(references);
            tester.TestState.AdditionalReferences.Add(typeof(GenerateAutomaticInterfaceAttribute).Assembly);

            tester.ExpectedDiagnostics.AddRange(new List<DiagnosticResult>() { new DiagnosticResult("AutomaticInterface", DiagnosticSeverity.Info), new DiagnosticResult("AutomaticInterface", DiagnosticSeverity.Info) });

            await tester.RunAsync();

        }

        [Fact]
        public async Task TestNoAttribute()
        {


            var code = @"
class C { }
";
            await new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { code },
                    ExpectedDiagnostics = { new DiagnosticResult("AutomaticInterface", DiagnosticSeverity.Info) }
        },

        }.RunAsync();

        }


        [Fact]
        public async Task GeneratesEmptyInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    [GenerateAutomaticInterface]
    class DemoClass
    {
                     }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
    }
}
";

            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task GeneratesStringPropertyInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        public string Hello { get; set; }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello { get; set; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task GeneratesStringPropertySetOnlyInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        private string x;
        public string Hello { set => x = value; }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello { set; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task GeneratesStringPropertyGetOnlyInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        private string x;
        public string Hello { get; }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello { get; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task AddsUsingsToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        public DirectoryInfo Hello { get; set; }
                     }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        System.IO.DirectoryInfo Hello { get; set; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task AddsPublicMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
                  
        public string Hello(){return """";}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello();
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task AddsPublicTaskMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;
using System.Threading.Tasks;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        public async Task<string> Hello(){return """";}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;
using System.Threading.Tasks;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        System.Threading.Tasks.Task<string> Hello();
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task AddsPublicWithParamsMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
                  
        public string Hello(string x){return x;}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello(string x);
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task AddsPublicWithParamsGenericMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;
using System.Threading.Tasks;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        public string Hello(Task<string> x){return """";}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;
using System.Threading.Tasks;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello(System.Threading.Tasks.Task<string> x);
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task AddsPublicWithMultipleParamsMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        public string Hello(string x, int y, double z){return x;}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello(string x, int y, double z);
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task IgnoresNotPublicMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        private string Hello(string x, int y, double z){return x;}
        internal string Hello2(string x, int y, double z){return x;}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task AddsDescriptionFromMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {

        /// <summary>
        /// TEST
        /// </summary>
        /// <returns></returns>
        public string Hello(string x){return x;}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        /// <summary>
        /// TEST
        /// </summary>
        /// <returns></returns>
        string Hello(string x);
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }


        [Fact]
        public async Task AddsMultiLineDescriptionFromMethodToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {

        /**
         * <summary>Hello World!</summary>
         */
        public string Hello(string x){return x;}
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System.IO;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        /**
        * <summary>Hello World!</summary>
        */
        string Hello(string x);
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task OmitsPrivateSetPropertyInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        public string Hello { get; private set; }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello { get; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task CopiesDocumentationOfPropertyToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{

    [GenerateAutomaticInterface]
    class DemoClass
    {
        /// <summary>
        /// Bla bla
        /// </summary>
        public string Hello { get; private set; }
                     }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        /// <summary>
        /// Bla bla
        /// </summary>
        string Hello { get; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task CopiesDocumentationOfClassToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
        /// <summary>
        /// Bla bla
        /// </summary>
    [GenerateAutomaticInterface]
    class DemoClass
    {
        public string Hello { get; private set; }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    /// <summary>
    /// Bla bla
    /// </summary>
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello { get; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task DoesNotCopyCtorToToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
        /// <summary>
        /// Bla bla
        /// </summary>
    [GenerateAutomaticInterface]
    class DemoClass
    {
        DemoClass(string x)
        {

        }

        public string Hello { get; private set; }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    /// <summary>
    /// Bla bla
    /// </summary>
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        string Hello { get; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task DoesNotCopyStaticMethodsToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
        /// <summary>
        /// Bla bla
        /// </summary>
    [GenerateAutomaticInterface]
    class DemoClass
    {
        public static string Hello => ""abc""; // property

        public static string StaticMethod()  // method
        {
            return ""static"";
       }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    /// <summary>
    /// Bla bla
    /// </summary>
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task MakesGenericInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
        /// <summary>
        /// Bla bla
        /// </summary>
    [GenerateAutomaticInterface]
    class DemoClass<T,U> where T:class
    {
        public string Hello { get; private set; }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;

namespace AutomaticInterfaceExample
{
    /// <summary>
    /// Bla bla
    /// </summary>
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass<T,U> where T:class
    {
        string Hello { get; }
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task CopiesEventsToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System;

namespace AutomaticInterfaceExample
{
        /// <summary>
        /// Bla bla
        /// </summary>
    [GenerateAutomaticInterface]
    class DemoClass
    {

        /// <summary>
        /// Bla bla
        /// </summary>
        public event EventHandler ShapeChanged;  // included

        private event EventHandler ShapeChanged2; // ignored because not public
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System;

namespace AutomaticInterfaceExample
{
    /// <summary>
    /// Bla bla
    /// </summary>
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        /// <summary>
        /// Bla bla
        /// </summary>
        event System.EventHandler ShapeChanged;
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task DoesNotCopyIndexerToInterface()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System;

namespace AutomaticInterfaceExample
{
        /// <summary>
        /// Bla bla
        /// </summary>
    [GenerateAutomaticInterface]
    class DemoClass
    {

        private int[] arr = new int[100];

        /// <summary>
        /// Bla bla
        /// </summary>
        public int this[int index] // currently ignored
        {
            get => arr[index];
            set => arr[index] = value;
        }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System;

namespace AutomaticInterfaceExample
{
    /// <summary>
    /// Bla bla
    /// </summary>
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }

        [Fact]
        public async Task FullExample()
        {
            var code = @"
using AutomaticInterfaceAttribute;
using System;

namespace AutomaticInterfaceExample
{
        /// <summary>
        /// Bla bla
        /// </summary>
    [GenerateAutomaticInterface]
    class DemoClass
    {
        /// <summary>
        /// Property Documentation will be copied
        /// </summary>
        public string Hello { get; set; }  // included, get and set are copied to the interface when public

        public string OnlyGet { get; } // included, get and set are copied to the interface when public

        /// <summary>
        /// Method Documentation will be copied
        /// </summary>
        public string AMethod(string x, string y) // included
        {
            return BMethod(x,y);
        }

        private string BMethod(string x, string y) // ignored because not public
        {
            return x + y;
        }

        public static string StaticProperty => ""abc""; // static property, ignored

        public static string StaticMethod()  // static method, ignored
            {
                return ""static"" + DateTime.Now;
            }

        /// <summary>
        /// event Documentation will be copied
        /// </summary>

        public event EventHandler ShapeChanged;  // included

        private event EventHandler ShapeChanged2; // ignored because not public

        private readonly int[] arr = new int[100];

        public int this[int index] // currently ignored
        {
            get => arr[index];
            set => arr[index] = value;
        }
    }
}
";

            var expected = @"//--------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using AutomaticInterfaceAttribute;
using System;

namespace AutomaticInterfaceExample
{
    /// <summary>
    /// Bla bla
    /// </summary>
    [GeneratedCode(""AutomaticInterface"", """")]
    public partial interface IDemoClass
    {
        /// <summary>
        /// Property Documentation will be copied
        /// </summary>
        string Hello { get; set; }
        
        string OnlyGet { get; }
        
        /// <summary>
        /// Method Documentation will be copied
        /// </summary>
        string AMethod(string x, string y);
        
        /// <summary>
        /// event Documentation will be copied
        /// </summary>
        event System.EventHandler ShapeChanged;
        
    }
}
";
            await RunTestAsync(code, expected);
            Assert.True(true); // silence warnings, real test happens in the RunAsync() method
        }
        // todo clean up generated source code
    }
}

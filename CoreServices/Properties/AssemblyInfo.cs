using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CoreServices")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("CoreServices")]
[assembly: AssemblyCopyright("Copyright © Semyon A. Chertkov 2008-2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3b8b224b-97dc-4665-a310-02076be0e02a")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("3.4.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Security 
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]

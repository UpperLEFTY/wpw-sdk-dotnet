﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Worldpay.Within.Sample.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Worldpay.Within.Sample.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;host&quot;: &quot;127.0.0.1&quot;,
        ///  &quot;port&quot;: 8778,
        ///  &quot;hceCard&quot;: {
        ///    &quot;firstName&quot;: &quot;Bilbo&quot;,
        ///    &quot;lastName&quot;: &quot;Baggins&quot;,
        ///    &quot;expMonth&quot;: 11,
        ///    &quot;expYear&quot;: 2018,
        ///    &quot;cardNumber&quot;: &quot;5555555555554444&quot;,
        ///    &quot;type&quot;: &quot;Card&quot;,
        ///    &quot;cvc&quot;: &quot;113&quot;
        ///  },
        ///  &quot;pspConfig&quot;: {
        ///    // Worldpay Online Payments
        ///    &quot;psp_name&quot;: &quot;worldpayonlinepayments&quot;,
        ///    &quot;api_endpoint&quot;: &quot;https://api.worldpay.com/v1&quot;
        ///
        ///    // Worldpay Total US / SecureNet
        ///    // &quot;PSP_NAME&quot;: &quot;securenet&quot;,
        ///    // &quot;API_ENDPOINT&quot;: &quot;https://gwapi.demo.securenet.com/api&quot;,
        ///    // &quot; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ConsumerConfig {
            get {
                return ResourceManager.GetString("ConsumerConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;host&quot;: &quot;127.0.0.1&quot;,
        ///  &quot;port&quot;: 9090,
        ///  &quot;pspConfig&quot;: {
        ///    &quot;psp_name&quot;: &quot;worldpayonlinepayments&quot;,
        ///    &quot;hte_public_key&quot;: &quot;T_C_97e8cbaa-14e0-4b1c-b2af-469daf8f1356&quot;,
        ///    &quot;hte_private_key&quot;: &quot;T_S_3bdadc9c-54e0-4587-8d91-29813060fecd&quot;,
        ///    &quot;api_endpoint&quot;: &quot;https://api.worldpay.com/v1&quot;,
        ///    &quot;merchant_client_key&quot;: &quot;T_C_97e8cbaa-14e0-4b1c-b2af-469daf8f1356&quot;,
        ///    &quot;merchant_service_key&quot;: &quot;T_S_3bdadc9c-54e0-4587-8d91-29813060fecd&quot;
        ///  }
        ///}
        ///.
        /// </summary>
        internal static string ProducerConfig {
            get {
                return ResourceManager.GetString("ProducerConfig", resourceCulture);
            }
        }
    }
}

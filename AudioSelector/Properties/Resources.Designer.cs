﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AudioSelector.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AudioSelector.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
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
        ///   (アイコン) に類似した型 System.Drawing.Icon のローカライズされたリソースを検索します。
        /// </summary>
        internal static System.Drawing.Icon appicon_black {
            get {
                object obj = ResourceManager.GetObject("appicon_black", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   (アイコン) に類似した型 System.Drawing.Icon のローカライズされたリソースを検索します。
        /// </summary>
        internal static System.Drawing.Icon appicon_white {
            get {
                object obj = ResourceManager.GetObject("appicon_white", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Hotkey registration failed.
        ///Please change the hotkey combination to be registered. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string HotKeyRegistrationError {
            get {
                return ResourceManager.GetString("HotKeyRegistrationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Alt に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string KeyAlt {
            get {
                return ResourceManager.GetString("KeyAlt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ctrl に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string KeyCtrl {
            get {
                return ResourceManager.GetString("KeyCtrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Shift に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string KeyShift {
            get {
                return ResourceManager.GetString("KeyShift", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Win に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string KeyWin {
            get {
                return ResourceManager.GetString("KeyWin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Disabled に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string SwitchDisabled {
            get {
                return ResourceManager.GetString("SwitchDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Enabled に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string SwitchEnabled {
            get {
                return ResourceManager.GetString("SwitchEnabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   AudioSelector
        ///Press {0} to show selector window. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string TaskbarToolTip {
            get {
                return ResourceManager.GetString("TaskbarToolTip", resourceCulture);
            }
        }
    }
}

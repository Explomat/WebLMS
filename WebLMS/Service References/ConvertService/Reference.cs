﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebLMS.ConvertService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UploadFileInfo", Namespace="http://service.weblms.ru")]
    [System.SerializableAttribute()]
    public partial class UploadFileInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] ByteArrayField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EmailField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] ByteArray {
            get {
                return this.ByteArrayField;
            }
            set {
                if ((object.ReferenceEquals(this.ByteArrayField, value) != true)) {
                    this.ByteArrayField = value;
                    this.RaisePropertyChanged("ByteArray");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Email {
            get {
                return this.EmailField;
            }
            set {
                if ((object.ReferenceEquals(this.EmailField, value) != true)) {
                    this.EmailField = value;
                    this.RaisePropertyChanged("Email");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResponseFileInfo", Namespace="http://service.weblms.ru")]
    [System.SerializableAttribute()]
    public partial class ResponseFileInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string HashField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PathField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Hash {
            get {
                return this.HashField;
            }
            set {
                if ((object.ReferenceEquals(this.HashField, value) != true)) {
                    this.HashField = value;
                    this.RaisePropertyChanged("Hash");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Path {
            get {
                return this.PathField;
            }
            set {
                if ((object.ReferenceEquals(this.PathField, value) != true)) {
                    this.PathField = value;
                    this.RaisePropertyChanged("Path");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://service.weblms.ru", ConfigurationName="ConvertService.IConverter")]
    public interface IConverter {
        
        // CODEGEN: Generating message contract since the wrapper name (DownloadRequest) of message DownloadRequest does not match the default value (DownloadFile)
        [System.ServiceModel.OperationContractAttribute(Action="http://service.weblms.ru/IConverter/DownloadFile", ReplyAction="http://service.weblms.ru/IConverter/DownloadFileResponse")]
        WebLMS.ConvertService.RemoteFileInfo DownloadFile(WebLMS.ConvertService.DownloadRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://service.weblms.ru/IConverter/DownloadFile", ReplyAction="http://service.weblms.ru/IConverter/DownloadFileResponse")]
        System.Threading.Tasks.Task<WebLMS.ConvertService.RemoteFileInfo> DownloadFileAsync(WebLMS.ConvertService.DownloadRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://service.weblms.ru/IConverter/ConvertFile", ReplyAction="http://service.weblms.ru/IConverter/ConvertFileResponse")]
        WebLMS.ConvertService.ResponseFileInfo ConvertFile(WebLMS.ConvertService.UploadFileInfo request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://service.weblms.ru/IConverter/ConvertFile", ReplyAction="http://service.weblms.ru/IConverter/ConvertFileResponse")]
        System.Threading.Tasks.Task<WebLMS.ConvertService.ResponseFileInfo> ConvertFileAsync(WebLMS.ConvertService.UploadFileInfo request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DownloadRequest", WrapperNamespace="http://service.weblms.ru", IsWrapped=true)]
    public partial class DownloadRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.weblms.ru", Order=0)]
        public string Path;
        
        public DownloadRequest() {
        }
        
        public DownloadRequest(string Path) {
            this.Path = Path;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RemoteFileInfo", WrapperNamespace="http://service.weblms.ru", IsWrapped=true)]
    public partial class RemoteFileInfo {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://service.weblms.ru")]
        public long Length;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://service.weblms.ru", Order=0)]
        public System.IO.Stream FileByteStream;
        
        public RemoteFileInfo() {
        }
        
        public RemoteFileInfo(long Length, System.IO.Stream FileByteStream) {
            this.Length = Length;
            this.FileByteStream = FileByteStream;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IConverterChannel : WebLMS.ConvertService.IConverter, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ConverterClient : System.ServiceModel.ClientBase<WebLMS.ConvertService.IConverter>, WebLMS.ConvertService.IConverter {
        
        public ConverterClient() {
        }
        
        public ConverterClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConverterClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConverterClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConverterClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WebLMS.ConvertService.RemoteFileInfo WebLMS.ConvertService.IConverter.DownloadFile(WebLMS.ConvertService.DownloadRequest request) {
            return base.Channel.DownloadFile(request);
        }
        
        public long DownloadFile(string Path, out System.IO.Stream FileByteStream) {
            WebLMS.ConvertService.DownloadRequest inValue = new WebLMS.ConvertService.DownloadRequest();
            inValue.Path = Path;
            WebLMS.ConvertService.RemoteFileInfo retVal = ((WebLMS.ConvertService.IConverter)(this)).DownloadFile(inValue);
            FileByteStream = retVal.FileByteStream;
            return retVal.Length;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WebLMS.ConvertService.RemoteFileInfo> WebLMS.ConvertService.IConverter.DownloadFileAsync(WebLMS.ConvertService.DownloadRequest request) {
            return base.Channel.DownloadFileAsync(request);
        }
        
        public System.Threading.Tasks.Task<WebLMS.ConvertService.RemoteFileInfo> DownloadFileAsync(string Path) {
            WebLMS.ConvertService.DownloadRequest inValue = new WebLMS.ConvertService.DownloadRequest();
            inValue.Path = Path;
            return ((WebLMS.ConvertService.IConverter)(this)).DownloadFileAsync(inValue);
        }
        
        public WebLMS.ConvertService.ResponseFileInfo ConvertFile(WebLMS.ConvertService.UploadFileInfo request) {
            return base.Channel.ConvertFile(request);
        }
        
        public System.Threading.Tasks.Task<WebLMS.ConvertService.ResponseFileInfo> ConvertFileAsync(WebLMS.ConvertService.UploadFileInfo request) {
            return base.Channel.ConvertFileAsync(request);
        }
    }
}

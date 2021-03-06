/**
 * Autogenerated by Thrift Compiler (0.10.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace Worldpay.Within.Rpc.Types
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class ServiceDetails : TBase
  {

    public int? ServiceId { get; set; }

    public string ServiceDescription { get; set; }

    public string ServiceName { get; set; }

    public ServiceDetails() {
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.I32) {
                ServiceId = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.String) {
                ServiceDescription = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.String) {
                ServiceName = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public void Write(TProtocol oprot) {
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct("ServiceDetails");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (ServiceId != null) {
          field.Name = "serviceId";
          field.Type = TType.I32;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(ServiceId.Value);
          oprot.WriteFieldEnd();
        }
        if (ServiceDescription != null) {
          field.Name = "serviceDescription";
          field.Type = TType.String;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(ServiceDescription);
          oprot.WriteFieldEnd();
        }
        if (ServiceName != null) {
          field.Name = "serviceName";
          field.Type = TType.String;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(ServiceName);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("ServiceDetails(");
      bool __first = true;
      if (ServiceId != null) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ServiceId: ");
        __sb.Append(ServiceId);
      }
      if (ServiceDescription != null) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ServiceDescription: ");
        __sb.Append(ServiceDescription);
      }
      if (ServiceName != null) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ServiceName: ");
        __sb.Append(ServiceName);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}

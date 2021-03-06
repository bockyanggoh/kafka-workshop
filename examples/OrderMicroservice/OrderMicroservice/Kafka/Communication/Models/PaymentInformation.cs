// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.7.7.5
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Kafka.Communication.Models
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	public partial class PaymentInformation : ISpecificRecord
	{
		public static Schema _SCHEMA = Schema.Parse("{\"type\":\"record\",\"name\":\"PaymentInformation\",\"namespace\":\"Kafka.Communication.Mod" +
				"els\",\"fields\":[{\"name\":\"PaymentId\",\"type\":\"string\"},{\"name\":\"PaymentStatus\",\"typ" +
				"e\":\"string\"},{\"name\":\"PaymentAmount\",\"type\":\"double\"}]}");
		private string _PaymentId;
		private string _PaymentStatus;
		private double _PaymentAmount;
		public virtual Schema Schema
		{
			get
			{
				return PaymentInformation._SCHEMA;
			}
		}
		public string PaymentId
		{
			get
			{
				return this._PaymentId;
			}
			set
			{
				this._PaymentId = value;
			}
		}
		public string PaymentStatus
		{
			get
			{
				return this._PaymentStatus;
			}
			set
			{
				this._PaymentStatus = value;
			}
		}
		public double PaymentAmount
		{
			get
			{
				return this._PaymentAmount;
			}
			set
			{
				this._PaymentAmount = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.PaymentId;
			case 1: return this.PaymentStatus;
			case 2: return this.PaymentAmount;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.PaymentId = (System.String)fieldValue; break;
			case 1: this.PaymentStatus = (System.String)fieldValue; break;
			case 2: this.PaymentAmount = (System.Double)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}

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
	
	public partial class DeliveryMessage : ISpecificRecord
	{
		public static Schema _SCHEMA = Schema.Parse(@"{""type"":""record"",""name"":""DeliveryMessage"",""namespace"":""Kafka.Communication.Models"",""fields"":[{""name"":""CorrelationId"",""doc"":""Correlation Request Id for this order"",""type"":""string""},{""name"":""RequestType"",""doc"":""Type of Request"",""type"":""string""},{""name"":""OrderIds"",""doc"":""Order Ids for this order"",""type"":{""type"":""array"",""items"":""string""}},{""name"":""Username"",""doc"":""Username of requestor"",""type"":""string""},{""name"":""PaymentStatus"",""doc"":""Payment Status for Order"",""type"":""string""},{""name"":""RequestedTs"",""doc"":""Timestamp of Request"",""type"":""string""}]}");
		/// <summary>
		/// Correlation Request Id for this order
		/// </summary>
		private string _CorrelationId;
		/// <summary>
		/// Type of Request
		/// </summary>
		private string _RequestType;
		/// <summary>
		/// Order Ids for this order
		/// </summary>
		private IList<System.String> _OrderIds;
		/// <summary>
		/// Username of requestor
		/// </summary>
		private string _Username;
		/// <summary>
		/// Payment Status for Order
		/// </summary>
		private string _PaymentStatus;
		/// <summary>
		/// Timestamp of Request
		/// </summary>
		private string _RequestedTs;
		public virtual Schema Schema
		{
			get
			{
				return DeliveryMessage._SCHEMA;
			}
		}
		/// <summary>
		/// Correlation Request Id for this order
		/// </summary>
		public string CorrelationId
		{
			get
			{
				return this._CorrelationId;
			}
			set
			{
				this._CorrelationId = value;
			}
		}
		/// <summary>
		/// Type of Request
		/// </summary>
		public string RequestType
		{
			get
			{
				return this._RequestType;
			}
			set
			{
				this._RequestType = value;
			}
		}
		/// <summary>
		/// Order Ids for this order
		/// </summary>
		public IList<System.String> OrderIds
		{
			get
			{
				return this._OrderIds;
			}
			set
			{
				this._OrderIds = value;
			}
		}
		/// <summary>
		/// Username of requestor
		/// </summary>
		public string Username
		{
			get
			{
				return this._Username;
			}
			set
			{
				this._Username = value;
			}
		}
		/// <summary>
		/// Payment Status for Order
		/// </summary>
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
		/// <summary>
		/// Timestamp of Request
		/// </summary>
		public string RequestedTs
		{
			get
			{
				return this._RequestedTs;
			}
			set
			{
				this._RequestedTs = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.CorrelationId;
			case 1: return this.RequestType;
			case 2: return this.OrderIds;
			case 3: return this.Username;
			case 4: return this.PaymentStatus;
			case 5: return this.RequestedTs;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.CorrelationId = (System.String)fieldValue; break;
			case 1: this.RequestType = (System.String)fieldValue; break;
			case 2: this.OrderIds = (IList<System.String>)fieldValue; break;
			case 3: this.Username = (System.String)fieldValue; break;
			case 4: this.PaymentStatus = (System.String)fieldValue; break;
			case 5: this.RequestedTs = (System.String)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
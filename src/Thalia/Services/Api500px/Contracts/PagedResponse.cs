using System.Runtime.Serialization;
using Thalia.Services.Api500.Interfaces;

namespace Thalia.Services.Api500.Contracts
{
	[DataContract]
	public abstract class PagedResponse: Response, IPagedResponse
	{
		#region Public Properties
		[DataMember(Name = "current_page")]
		public virtual int CurrentPage { get; set; }

		[DataMember(Name = "total_items")]
		public virtual int TotalItems { get; set; }

		[DataMember(Name = "total_pages")]
		public virtual int TotalPages { get; set; }
		#endregion
	}
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialNetworkApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment_Like
    {
        public int comment_id { get; set; }
        public int profile_id { get; set; }
        public System.DateTime timestamp { get; set; }
        public byte read { get; set; }
    
        public virtual Comment Comment { get; set; }
        public virtual Profile Profile { get; set; }
    }
}

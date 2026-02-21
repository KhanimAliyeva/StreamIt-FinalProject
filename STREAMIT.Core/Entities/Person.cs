using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.Core.Entities
{
    public class Person:BaseEntity
    {
        public string ImageUrl { get; set; }= string.Empty;
        public string Name { get; set; }=string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Born { get; set; }
        public DateTime? Died { get; set; }
        public float Popularity { get; set; }
        public string Biography { get; set; } = string.Empty;

        public ICollection<MoviePerson> MoviePeople { get; set; } = [];
        public ICollection<TvShowPerson> TVShowPeople { get; set; }=[];
    }


}

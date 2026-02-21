using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using STREAMIT.DataAccess.Repositories.Abstractions;
using STREAMIT.DataAccess.Repositories.Implementations.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Repositories.Implementations
{
    internal class PersonRepository(AppDbContext _context) : Repository<Person>(_context), IPersonRepository
    {
    }
}

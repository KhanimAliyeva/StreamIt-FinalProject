using STREAMIT.Core.Entities;
using STREAMIT.DataAccess.Contexts;
using STREAMIT.DataAccess.Repositories.Abstractions;
using STREAMIT.DataAccess.Repositories.Implementations.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace STREAMIT.DataAccess.Repositories.Implementations
{
    internal class LanguageRepository(AppDbContext _context) : Repository<Language>(_context), ILanguageRepository
    {
    }
}

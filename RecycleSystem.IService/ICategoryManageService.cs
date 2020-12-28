using RecycleSystem.Data.Data.CategoryDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecycleSystem.IService
{
   public interface ICategoryManageService
    {
        IEnumerable<CategoryOutput> GetCategories(int page, int limit, out int count, string queryInfo);
        IEnumerable<CategoryOutput> GetCategories();
        CategoryOutput GetCategory(int id);
        bool AddCategory(CategoryInput categoryInput,out string msg);
        bool BanCategory(int id, out string msg);
        bool UpdateCategoryById(CategoryInput categoryInput, out string msg);
    }
}

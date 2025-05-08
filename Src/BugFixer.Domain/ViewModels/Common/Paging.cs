using Microsoft.EntityFrameworkCore;

namespace BugFixer.Domain.ViewModels.Common
{
    public class Paging<T>
    {
        public Paging()
        {
            CurrentPage = 1;
            HowManyShowPageBeforeAfter = 3;
            TakeEntityToShow = 5;
            Entities = new List<T>();
        }

        public int StartPage { get; set; }

        public int CurrentPage { get; set; }

        public int EndPage { get; set; }

        public int TotalPage { get; set; }

        public int HowManyShowPageBeforeAfter { get; set; }

        public int AllEntityCount { get; set; }

        public int TakeEntityToShow { get; set; }

        public int SkipEntity { get; set; }

        public List<T> Entities { get; set; }

        public PagingViewModel GetPaging()
        {
            return new PagingViewModel
            {
                StartPage = StartPage,
                CurrentPage = CurrentPage,
                EndPage = EndPage
            };
        }

        public async Task SetPaging(IQueryable<T> query)
        {
            // Calculate the total number of entities
            AllEntityCount = query.Count();
            // Calcute the total number of pages
            TotalPage = (int)Math.Ceiling(AllEntityCount / (double)TakeEntityToShow);
            // When Clint request page is less than 1 
            CurrentPage = CurrentPage < 1 ? 1 : CurrentPage;
            // When Clint request page is more than total page
            CurrentPage = CurrentPage > TotalPage ? TotalPage : CurrentPage;
            // Calcute for show Entity At current page 
            SkipEntity = (CurrentPage - 1) * TakeEntityToShow;

            StartPage = CurrentPage - HowManyShowPageBeforeAfter > 0 ? CurrentPage - HowManyShowPageBeforeAfter : 1;

            EndPage = CurrentPage + HowManyShowPageBeforeAfter > TotalPage ? TotalPage : CurrentPage + HowManyShowPageBeforeAfter;

            Entities = await query.Skip(SkipEntity).Take(TakeEntityToShow).ToListAsync();

        }
    }

    public class PagingViewModel
    {
        public int StartPage { get; set; }

        public int CurrentPage { get; set; }

        public int EndPage { get; set; }
    }
}

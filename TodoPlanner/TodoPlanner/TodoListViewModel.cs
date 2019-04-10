using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace TodoPlanner
{
    class TodoListViewModel : BaseFodyObservable
    {
        public TodoListViewModel(INavigation navigation)
        {
            _navigation = navigation;
            AddItem = new Command(HandleAddItem);

            GetGroupedTodoList().ContinueWith(t =>
            {
                GroupedTodoList = t.Result;
            });
            Delete = new Command<TodoItem>(HandleDelete);
            ChangeIsCompleted = new Command<TodoItem>(HandleChangeIsCompleted);
        }

        private INavigation _navigation;
        public Command AddItem { get; set; }
        public ILookup<string, TodoItem> GroupedTodoList { get; set; }
        public string Title => "My Todo List";
        private List<TodoItem> _todoList = new List<TodoItem>
        {
            new TodoItem { Id = 0, Title = "Create First Todo", IsCompleted = true},
            new TodoItem { Id = 1, Title = "Run a Marathon"},
            new TodoItem { Id = 2, Title = "Create TodoXamarinForms blog post"},
        };

        private async Task<ILookup<string, TodoItem>> GetGroupedTodoList()
        {
            return (await App.TodoRepository.GetList())
                                .OrderBy(t => t.IsCompleted)
                                .ToLookup(t => t.IsCompleted ? "Completed" : "Active");
        }

        /**
         * Delete a Tasks
         */
        public Command<TodoItem> Delete { get; set; }
        public async void HandleDelete(TodoItem itemToDelete)
        {
            await App.TodoRepository.DeleteItem(itemToDelete);
            // Update displayed list
            GroupedTodoList = await GetGroupedTodoList();
        }

        /**
         * Complete a tasks
         */
        public Command<TodoItem> ChangeIsCompleted { get; set; }
        public async void HandleChangeIsCompleted(TodoItem itemToUpdate)
        {
            await App.TodoRepository.ChangeItemIsCompleted(itemToUpdate);
            // Update displayed list
            GroupedTodoList = await GetGroupedTodoList();
        }

        /**
         * Handle Item
         */
        public async void HandleAddItem()
        {
            await _navigation.PushModalAsync(new AddTodoItem());
        }

        /**
         * Refreshed the list when an new item is added
         */
        public async Task RefreshTaskList()
        {
            GroupedTodoList = await GetGroupedTodoList();
        }
    }
}

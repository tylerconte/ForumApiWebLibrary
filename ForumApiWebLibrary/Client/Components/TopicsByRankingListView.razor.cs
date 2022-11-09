﻿using ForumApiDataService.Client;
using ForumApiDataService.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Telerik.Blazor.Components;

namespace ForumApiWebLibrary.Client.Components
{
    public partial class TopicsByRankingListView : ComponentBase
    {
        [CascadingParameter]
        public long FId { get; set; }

        [CascadingParameter]
        public int ActiveTabIndex { get; set; }

        //[CascadingParameter]
        public TopicsModel TopicsModel { get; set; } = new TopicsModel();

        public int PageSize { get; set; } = 10;

        //public TopicsModel TopicsModel { get; set; } = new TopicsModel();

        protected override async Task OnInitializedAsync()
        {
            await FetchData();

            await base.OnInitializedAsync();
        }

        private async Task FetchData()
        {
            switch (ActiveTabIndex)
            {
                case 0:
                    TopicsModel = await ForumApiClient.GetTopicsActiveViewAsync(FId, 1, PageSize);
                    break;
                case 1:
                    TopicsModel = await ForumApiClient.GetTopicsRecentViewAsync(FId, 1, PageSize);
                    break;
                case 2:
                    TopicsModel = await ForumApiClient.GetTopicsUpCountViewAsync(FId, 1, PageSize);
                    break;
            }
        }

        async Task PageChangedHandler(int currPageIndex)
        {
            //result = $"The user is now on page {currPageIndex}";
        }

        //public EventCallback<ListViewCommandEventArgs> OnClick { get; set; }
        async Task ReplyHandler(ListViewCommandEventArgs args)
        {
            //result = $"The user is now on page {currPageIndex}";
            NavigationManager.NavigateTo("/verify");
        }

        async Task UpdateHandler(ListViewCommandEventArgs args)
        {
            TopicModel item = (TopicModel)args.Item;

            //// perform actual data source operation here through your service
            //await MyService.Update(item);
            var response = await ForumApiClient.PutTopicAsync(item);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    break;
                case HttpStatusCode.BadRequest:
                    string res = await response.Content.ReadAsStringAsync();
                    //do something with the error
                    break;
                case HttpStatusCode.Unauthorized:
                    await ForumAuthService.Logout();
                    NavigationManager.NavigateTo("/login");
                    break;
                default:
                    return;
            }

            //// update the local view-model data with the service data
            //await GetListViewData();
            int index = TopicsModel.TopicsSortedByRanking.FindIndex(itm => itm.TId == item.TId);
            if (index > -1)
            {
                TopicsModel.TopicsList[index] = item;
            }

        }

        async Task DeleteHandler(ListViewCommandEventArgs args)
        {
            TopicModel item = (TopicModel)args.Item;

            // perform actual data source operation here through your service
            //await MyService.Delete(item);

            //// update the local view-model data with the service data
            //await GetListViewData();
        }

        async Task CreateHandler(ListViewCommandEventArgs args)
        {
            TopicModel item = (TopicModel)args.Item;

            var response = await ForumApiClient.PostTopicAsync(item);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    break;
                case HttpStatusCode.BadRequest:
                    string res = await response.Content.ReadAsStringAsync();
                    //do something with the error
                    break;
                case HttpStatusCode.Unauthorized:
                    await ForumAuthService.Logout();
                    NavigationManager.NavigateTo("/login");
                    break;
                default:
                    return;
            }

            //TopicsModel.TopicsList.Insert(0, item);

            await FetchData();
        }

        async Task EditHandler(ListViewCommandEventArgs e)
        {
            TopicModel currItem = e.Item as TopicModel;

            // prevent opening an item for editing on condition
            //if (currItem.Id < 2)
            //{
            //    e.IsCancelled = true;
            //}
        }

        public TopicModel OnModelInitHandler()
        {
            return new TopicModel(FId, String.Empty, String.Empty);
        }

    }
}

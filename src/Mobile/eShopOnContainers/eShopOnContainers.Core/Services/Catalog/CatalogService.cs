﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using eShopOnContainers.Core.Models.Catalog;
using eShopOnContainers.Core.Services.RequestProvider;
using eShopOnContainers.Core.Extensions;
using System.Collections.Generic;
using eShopOnContainers.ViewModels.Base;
using eShopOnContainers.Core.Helpers;
using Xamarin.Forms;
using eShopOnContainers.Core.Services.IPAddress;
using System.Text.RegularExpressions;

namespace eShopOnContainers.Core.Services.Catalog
{
    public class CatalogService : ICatalogService
    {
        private readonly IRequestProvider _requestProvider;

        public CatalogService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<ObservableCollection<CatalogItem>> FilterAsync(int catalogBrandId, int catalogTypeId)
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.CatalogEndpoint);

                builder.Path = string.Format("api/v1/catalog/items/type/{0}/brand/{1}", catalogTypeId, catalogBrandId);

                string uri = builder.ToString();

                CatalogRoot catalog =
                        await _requestProvider.GetAsync<CatalogRoot>(uri);

                if (catalog?.Data != null)
                    return catalog?.Data.ToObservableCollection();
                else
                    return new ObservableCollection<CatalogItem>();
            }
            catch
            {
                return new ObservableCollection<CatalogItem>();
            }
        }

        public async Task<ObservableCollection<CatalogItem>> GetCatalogAsync()
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.CatalogEndpoint);

                builder.Path = "api/v1/catalog/items";

                string uri = builder.ToString();

                CatalogRoot catalog =
                    await _requestProvider.GetAsync<CatalogRoot>(uri);

                if (catalog?.Data != null)
                {
                    // TODO: Experimental
                    if (!ViewModelLocator.Instance.UseMockService && Settings.UrlBase != GlobalSetting.DefaultEndpoint)
                    {
                        foreach (var catalogItem in catalog?.Data)
                        {
                            //string ipaddress = DependencyService.Get<IIPAddressService>().GetIPAddress();
                            Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                            MatchCollection result = ip.Matches(catalogItem.PictureUri);

                            if (result.Count != -1)
                            {
                                var serviceIp = result[0].Value;
                                catalogItem.PictureUri = catalogItem.PictureUri.Replace(serviceIp, Settings.UrlBase);
                            }
                        }
                    }

                    return catalog?.Data.ToObservableCollection();
                }
                else
                    return new ObservableCollection<CatalogItem>();
            }
            catch
            {
                return new ObservableCollection<CatalogItem>();
            }
        }

        public Task<CatalogItem> GetCatalogItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ObservableCollection<CatalogBrand>> GetCatalogBrandAsync()
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.CatalogEndpoint);

                builder.Path = "api/v1/catalog/catalogbrands";

                string uri = builder.ToString();

                IEnumerable<CatalogBrand> brands =
                       await _requestProvider.GetAsync<IEnumerable<CatalogBrand>>(uri);

                if (brands != null)
                    return brands?.ToObservableCollection();
                else
                    return new ObservableCollection<CatalogBrand>();
            }
            catch
            {
                return new ObservableCollection<CatalogBrand>();
            }
        }

        public async Task<ObservableCollection<CatalogType>> GetCatalogTypeAsync()
        {
            try
            {
                UriBuilder builder = new UriBuilder(GlobalSetting.Instance.CatalogEndpoint);

                builder.Path = "api/v1/catalog/catalogtypes";

                string uri = builder.ToString();

                IEnumerable<CatalogType> types =
                       await _requestProvider.GetAsync<IEnumerable<CatalogType>>(uri);

                if (types != null)
                    return types.ToObservableCollection();
                else
                    return new ObservableCollection<CatalogType>();
            }
            catch
            {
                return new ObservableCollection<CatalogType>();
            }
        }
    }
}

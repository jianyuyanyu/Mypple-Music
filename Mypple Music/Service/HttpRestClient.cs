﻿using Mypple_Music.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Service
{
    public class HttpRestClient
    {
        private readonly string apiUrl;
        protected readonly RestClient client;
        public HttpRestClient(string apiUrl)
        {
            this.apiUrl = apiUrl;
            client = new RestClient(apiUrl);
        }

        public async Task<Uri> UploadAsync(BaseRequest baseRequest)
        {          
            var url = new Uri(apiUrl + baseRequest.Route);
            var request = new RestRequest(url, baseRequest.Method);
            request.AlwaysMultipartFormData = true;
            if (baseRequest.Parameter != null)
            {
                request.AddFile("File",baseRequest.Parameter.ToString());
            }
            //request.AddHeader("Authorization", "Bearer " + token);//增加的 JWT 认证
            RestResponse response = await client.ExecuteAsync(request);          
            return JsonConvert.DeserializeObject<Uri>(response.Content);
        }

        public async Task<T> ExecuteAsync<T>(BaseRequest baseRequest)
        {          
            var url = new Uri(apiUrl + baseRequest.Route);
            var request = new RestRequest(url, baseRequest.Method);
            request.AddHeader("Content-Type", baseRequest.ContentType);
            if (baseRequest.Parameter != null)
            {
                request.AddJsonBody(baseRequest.Parameter);
            }
            RestResponse response = await client.ExecuteAsync(request);          
            return JsonConvert.DeserializeObject<T>(response.Content);

        }

    }
}

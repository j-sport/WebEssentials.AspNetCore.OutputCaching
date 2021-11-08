﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using WebEssentials.AspNetCore.OutputCaching;
using Microsoft.Net.Http.Headers;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOutputCachingService _outputCachingService;
        private readonly IOutputCacheKeysProvider _outputCacheKeysProvider;

        public HomeController(IOutputCachingService outputCachingService, IOutputCacheKeysProvider outputCacheKeysProvider)
        {
            this._outputCachingService = outputCachingService;
            this._outputCacheKeysProvider = outputCacheKeysProvider;
        }

        [OutputCache(Duration = 600)]
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Api()
        {
            HttpContext.EnableOutputCaching(TimeSpan.FromMinutes(1));
            return View("Index");
        }

        [OutputCache(Duration = 600, VaryByParam = "foo")]
        public IActionResult Query()
        {
            return View("Index");
        }

        [OutputCache(VaryByCustom = "foo")]
        public IActionResult Custom()
        {
            return View("Index");
        }

        [OutputCache(Profile = "default")]
        public IActionResult Profile()
        {
            return View("Index");
        }

        public IActionResult InvalidateQueryCache()
        {
            var key = _outputCacheKeysProvider.GetRequestCacheKey(HttpContext, 
                                                                  new OutputCacheProfile() { 
                                                                      Duration = 600, 
                                                                      VaryByParam = "foo", 
                                                                      VaryByHeader = null,
                                                                      VaryByCustom = null
                                                                  }, 
                                                                  Url.Action("Query", "Home"), 
                                                                  System.Net.WebRequestMethods.Http.Get);
            _outputCachingService.Remove(key);
            return View("Index");
        }

        public IActionResult Redirect()
        {
            Response.Headers.Add(HeaderNames.CacheControl, "no-store, no-cache, must-revalidate");
            return RedirectToActionPermanent("Index");
        }

        public IActionResult NoCache()
        {
            Response.Headers.Add(HeaderNames.CacheControl, "no-store, no-cache, must-revalidate");
            return View("Index");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HackAtHome.Entities;

namespace HackAtHomeClient.Fragments
{
    public class EvidenceDetailFragment: Fragment
    {
        public string Token { get; set; }

        public string FullName { get; set; }

        public Evidence Evidence { get; set; }

        public EvidenceDetail Detail { get; set; }

        public override string ToString()
        {
            return $"{Evidence.Title} - {Evidence.Status}";
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }


    }
}
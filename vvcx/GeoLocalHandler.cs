﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Net;
using System.Xml.XPath;

namespace vvcx
{
    // pozostalosc po pierwotnym założeniu, chcesz to dowolnie to zmodyfikuj/zastąp czymś innym.

    class GeoLocalHandler
    {
        private static string urlPrefix = "http://dev.virtualearth.net/REST/v1/Locations?";
        private static string urlCityPart = "&locality=";
        private static string urlStreetPart = "&addressLine=";
        private static string urlXmlOutputPart = "&output=xml";
        private static string urlKeyPart = "&key=";
        private static string urlSpace = "%20";
        private static string urlLatitude = "Latitude";
        private static string urlLongitude = "Longitude";

        private static string routeUrlPrefix = "http://dev.virtualearth.net/REST/v1/Routes?";
        private static string wayPointPart = "&wayPoint.";
        private static string travelDuration = "TravelDuration";

        private string bingMapsKey;

        public GeoLocalHandler(string bingKey)
        {
            bingMapsKey = bingKey;
        }

        public Tuple<double, double> getLocationPoint(string city, string street)
        {
            replaceSpaces(city);
            replaceSpaces(street);
            string url = urlPrefix + urlCityPart + city + urlStreetPart + street + urlXmlOutputPart + urlKeyPart + bingMapsKey;
            Console.WriteLine(url);
            XmlDocument response = getXmlResponse(url);
            XmlNodeList latitude = response.GetElementsByTagName(urlLatitude);
            XmlNodeList longitude = response.GetElementsByTagName(urlLongitude);
            double latitueAsDouble, longitudeAsDouble;
            try
            {
                latitueAsDouble = Double.Parse(longitude[0].InnerText.Replace(".", ","));
                longitudeAsDouble = Double.Parse(latitude[0].InnerText.Replace(".", ","));
            }
            catch
            {
                latitueAsDouble = Double.Parse(longitude[0].InnerText);
                longitudeAsDouble = Double.Parse(latitude[0].InnerText);
            }

            Tuple<double, double> location = null;
            try
            {
                location = new Tuple<double, double>(longitudeAsDouble, latitueAsDouble);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: nie można uzyskać longitudeAsDouble i latitueAsDouble " + e);
            }
            return location;
        }

        public List<GeoRouteNode> getRoutes(List<Shop> geoPoints)
        {
            List<GeoRouteNode> routeNodes = new List<GeoRouteNode>();

            for (int i = 0; i < (geoPoints.Count() - 1); i++)
            {
                new Thread(() =>
                {
                    for (int j = (i + 1); j <= (geoPoints.Count() - 1); j++)
                    {
                        if (i == j)
                            continue;
                        string url = routeUrlPrefix + wayPointPart + Convert.ToString(1) + "=" + Convert.ToString(geoPoints[i].Latitude).Replace(',', '.') + "," + Convert.ToString(geoPoints[i].Longitude).Replace(',', '.') + wayPointPart + Convert.ToString(2) + "=" + Convert.ToString(geoPoints[j].Latitude).Replace(',', '.') + "," + Convert.ToString(geoPoints[j].Longitude).Replace(',', '.') + urlKeyPart + bingMapsKey + urlXmlOutputPart;
                        XmlDocument response = getXmlResponse(url);
                        XmlNodeList duration = response.GetElementsByTagName(travelDuration);
                        GeoRouteNode routeNode = new GeoRouteNode(geoPoints[i], geoPoints[j], Convert.ToDouble(duration[0].InnerText));
                        routeNodes.Add(routeNode);
                    }
                }).Start();
            }

            return routeNodes;
        }

        public double[][] getDistanceMatrix(List<Shop> geoPoints)
        {
            double[][] distanceMatrix = new double[geoPoints.Count][];
            List<Thread> Threads = new List<Thread>();
            for (int i = 0; i < geoPoints.Count(); i++)
            {
                int k = i;
                Thread thread = new Thread(() =>
                {
                    ThreadWork(k, ref distanceMatrix, geoPoints);
                });
                Threads.Add(thread);
                thread.Start();
            }
            foreach (Thread thread in Threads)
            {
                thread.Join();
            }
            return distanceMatrix;
        }
        void ThreadWork(int i, ref double[][] distanceMatrix, List<Shop> geoPoints)
        {
            distanceMatrix[i] = new double[geoPoints.Count];
            for (int j = 0; j < geoPoints.Count(); j++)
            {
                if (i == j)
                    continue;
                Console.WriteLine($"{geoPoints[i].Name} - {geoPoints[j].Name}");
                string url = routeUrlPrefix + wayPointPart + Convert.ToString(1) + "=" + Convert.ToString(geoPoints[i].Latitude).Replace(',', '.') + "," + Convert.ToString(geoPoints[i].Longitude).Replace(',', '.') + wayPointPart + Convert.ToString(2) + "=" + Convert.ToString(geoPoints[j].Latitude).Replace(',', '.') + "," + Convert.ToString(geoPoints[j].Longitude).Replace(',', '.') + urlKeyPart + bingMapsKey + urlXmlOutputPart;
                XmlDocument response = getXmlResponse(url);
                XmlNodeList duration = response.GetElementsByTagName(travelDuration);
                distanceMatrix[i][j] = Convert.ToDouble(duration[0].InnerText);
            }
        }

        private XmlDocument getXmlResponse(string url)
        {
            System.Diagnostics.Trace.WriteLine("Request URL (XML): " + url);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return xmlDoc;
            }
        }

        private void replaceSpaces(string text)
        {
            text.Replace(" ", urlSpace);
        }

    }
}

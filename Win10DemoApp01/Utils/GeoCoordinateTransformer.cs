using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win10DemoApp01.Utils
{
    /// <summary>
    /// 原程式碼參考 http://wangshifuola.blogspot.com/2010/08/twd97wgs84-wgs84twd97.html
    /// </summary>
    public class GeoCoordinateTransformer
    {

        public GeoCoordinateTransformer()
        {
        }

        public static double DMSToDouble(string inDMS)
        {
            string[] DMS = inDMS.Split(',');
            return Double.Parse(DMS[0]) + Double.Parse(DMS[1]) / 60 + Double.Parse(DMS[2]) / 3600;
        }


        static double a = 6378137.0;
        static double b = 6356752.314245;
        static double lon0 = 121 * Math.PI / 180;
        static double k0 = 0.9999;
        static int dx = 250000;

        //給WGS84經緯度度分秒轉成TWD97坐標    
        public static string lonlat_To_twd97(int lonD,int lonM,int lonS,int latD,int latM,int latS)
        {
            double RadianLon = (double)(lonD) + (double)lonM / 60 + (double)lonS / 3600;
            double RadianLat = (double)(latD) + (double)latM / 60 + (double)latS / 3600;
            return Cal_lonlat_To_twd97(RadianLon, RadianLat);
        }
        //給WGS84經緯度弧度轉成TWD97坐標    
        public static string lonlat_To_twd97(double RadianLon, double RadianLat)
        {
            return Cal_lonlat_To_twd97(RadianLon, RadianLat);
        }
        //給TWD97坐標 轉成 WGS84 度分秒字串  (type1傳度分秒   2傳弧度)    
        public static string TWD97_To_lonlat(double XValue, double YValue, int Type)
        {
            string lonlat = "";
            if (Type == 1)
            {
                string[] Answer = Cal_TWD97_To_lonlat(XValue, YValue).Split(',');
                int LonDValue = (int)double.Parse(Answer[0]);
                int LonMValue = (int)((double.Parse(Answer[0]) - LonDValue) * 60);
                int LonSValue = (int)((((double.Parse(Answer[0]) - LonDValue) * 60) - LonMValue) * 60);
                int LatDValue = (int)double.Parse(Answer[1]);
                int LatMValue = (int)((double.Parse(Answer[1]) - LatDValue) * 60);
                int LatSValue = (int)((((double.Parse(Answer[1]) - LatDValue) * 60) - LatMValue) * 60);
                lonlat = LonDValue + "度" + LonMValue + "分" + LonSValue + "秒," + LatDValue + "度" + LatMValue + "分" + LatSValue + "秒,";
            }
            else if (Type == 2)
            {
                lonlat = Cal_TWD97_To_lonlat(XValue, YValue);
            }
            return lonlat;
        }

        private static string Cal_lonlat_To_twd97(double lon ,double lat)
        {
            string TWD97 = "";
            lon = (lon/180) * Math.PI;
            lat = (lat/180) * Math.PI;
            //---------------------------------------------------------        
            double e = Math.Pow((1 - Math.Pow(b,2) / Math.Pow(a,2)), 0.5);
            double e2 = Math.Pow(e,2)/(1-Math.Pow(e,2));
            double n = ( a - b ) / ( a + b );
            double nu = a / Math.Pow((1-(Math.Pow(e,2)) * (Math.Pow(Math.Sin(lat), 2) ) ) , 0.5);
            double p = lon - lon0;        double A = a * (1 - n + (5/4) * (Math.Pow(n,2) - Math.Pow(n,3)) + (81/64) * (Math.Pow(n, 4)  - Math.Pow(n, 5)));
            double B = (3 * a * n/2.0) * (1 - n + (7/8.0)*(Math.Pow(n,2) - Math.Pow(n,3)) + (55/64.0)*(Math.Pow(n,4) - Math.Pow(n,5)));
            double C = (15 * a * (Math.Pow(n,2))/16.0)*(1 - n + (3/4.0)*(Math.Pow(n,2) - Math.Pow(n,3)));
            double D = (35 * a * (Math.Pow(n,3))/48.0)*(1 - n + (11/16.0)*(Math.Pow(n,2) - Math.Pow(n,3)));
            double E = (315 * a * (Math.Pow(n,4))/51.0)*(1 - n);
            double S = A * lat - B * Math.Sin(2 * lat) +C * Math.Sin(4 * lat) - D * Math.Sin(6 * lat) + E * Math.Sin(8 * lat);

            //計算Y值        
            double K1 = S*k0;
            double K2 = k0*nu*Math.Sin(2*lat)/4.0;
            double K3 = (k0*nu*Math.Sin(lat)*(Math.Pow(Math.Cos(lat),3))/24.0) * (5 - Math.Pow(Math.Tan(lat),2) + 9*e2*Math.Pow((Math.Cos(lat)),2) + 4*(Math.Pow(e2,2))*(Math.Pow(Math.Cos(lat),4)));
            double y = K1 + K2*(Math.Pow(p,2)) + K3*(Math.Pow(p,4));
            //計算X值        
            double K4 = k0*nu*Math.Cos(lat);
            double K5 = (k0*nu*(Math.Pow(Math.Cos(lat),3))/6.0) * (1 - Math.Pow(Math.Tan(lat),2) + e2*(Math.Pow(Math.Cos(lat),2)));
            double x = K4 * p + K5 * (Math.Pow(p, 3)) + dx;
            TWD97 = x.ToString()+ "," + y.ToString();
            return TWD97;
        }
        public static string Cal_TWD97_To_lonlat(double x, double y)
        {
            double dy = 0;
            double e = Math.Pow((1- Math.Pow(b,2)/Math.Pow(a,2)), 0.5);
            x -= dx;
            y -= dy;
            // Calculate the Meridional Arc        
            double M = y/k0;
            // Calculate Footprint Latitude        
            double mu = M/(a*(1.0 - Math.Pow(e, 2)/4.0 - 3*Math.Pow(e, 4)/64.0 - 5*Math.Pow(e, 6)/256.0));
            double e1 = (1.0 - Math.Pow((1.0 - Math.Pow(e, 2)), 0.5)) / (1.0 + Math.Pow((1.0 - Math.Pow(e, 2)), 0.5));
            double J1 = (3*e1/2 - 27*Math.Pow(e1, 3)/32.0);
            double J2 = (21*Math.Pow(e1, 2)/16 - 55*Math.Pow(e1, 4)/32.0);
            double J3 = (151*Math.Pow(e1, 3)/96.0);
            double J4 = (1097*Math.Pow(e1, 4)/512.0);
            double fp = mu + J1*Math.Sin(2*mu) + J2*Math.Sin(4*mu) + J3*Math.Sin(6*mu) + J4*Math.Sin(8*mu);
            // Calculate Latitude and Longitude        
            double e2 = Math.Pow((e*a/b), 2);
            double C1 = Math.Pow(e2*Math.Cos(fp), 2);
            double T1 = Math.Pow(Math.Tan(fp), 2);
            double R1 = a*(1-Math.Pow(e, 2))/Math.Pow((1-Math.Pow(e, 2)*Math.Pow(Math.Sin(fp), 2)), (3.0/2.0));
            double N1 = a/Math.Pow((1-Math.Pow(e, 2)*Math.Pow(Math.Sin(fp), 2)), 0.5);
            double D = x/(N1*k0);
            // 計算緯度        
            double Q1 = N1*Math.Tan(fp)/R1;
            double Q2 = (Math.Pow(D, 2)/2.0);
            double Q3 = (5 + 3*T1 + 10*C1 - 4*Math.Pow(C1, 2) - 9*e2)*Math.Pow(D, 4)/24.0;
            double Q4 = (61 + 90*T1 + 298*C1 + 45*Math.Pow(T1, 2) - 3*Math.Pow(C1, 2) - 252*e2)*Math.Pow(D, 6)/720.0;
            double lat = fp - Q1*(Q2 - Q3 + Q4);
            // 計算經度       
            double Q5 = D;
            double Q6 = (1 + 2*T1 + C1)*Math.Pow(D, 3)/6;
            double Q7 = (5 - 2*C1 + 28*T1 - 3*Math.Pow(C1, 2) + 8*e2 + 24*Math.Pow(T1, 2))*Math.Pow(D, 5)/120.0;
            double lon = lon0 + (Q5 - Q6 + Q7)/Math.Cos(fp);
            lat = (lat * 180) / Math.PI;
            //緯        
            lon = (lon * 180) / Math.PI;
            //經        
            string lonlat = lon + "," + lat;
            return lonlat;
        }

 
     }
}

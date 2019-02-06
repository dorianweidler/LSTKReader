using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace LSTKReader
{
    class Einsatz
    {
        private string einsatznummer;
        private string strasse;
        private string hausnummer;
        private string plz;
        private string ort;
        private string ortsteil;
        private string objekt;
        private string unterobjekt;
        private string koordinate;
        private string einsatzart;
        private string einsatzstichwort;
        private string stichwortmemo;
        private string diagnose;
        private string bemerkung;
        private string meldender;
        private string telefon;
        private string einsatztext;

        public string Einsatznummer { get => einsatznummer; set => einsatznummer = value; }
        public string Strasse { get => strasse; set => strasse = value; }
        public string Hausnummer { get => hausnummer; set => hausnummer = value; }
        public string Plz { get => plz; set => plz = value; }
        public string Ort { get => ort; set => ort = value; }
        public string Ortsteil { get => ortsteil; set => ortsteil = value; }
        public string Objekt { get => objekt; set => objekt = value; }
        public string Unterobjekt { get => unterobjekt; set => unterobjekt = value; }
        public string Koordinate { get => koordinate; set => koordinate = value; }
        public string Einsatzart { get => einsatzart; set => einsatzart = value; }
        public string Einsatzstichwort { get => einsatzstichwort; set => einsatzstichwort = value; }
        public string Stichwortmemo { get => stichwortmemo; set => stichwortmemo = value; }
        public string Diagnose { get => diagnose; set => diagnose = value; }
        public string Bemerkung { get => bemerkung; set => bemerkung = value; }
        public string Meldender { get => meldender; set => meldender = value; }
        public string Telefon { get => telefon; set => telefon = value; }
        public string Einsatztext { get => einsatztext; set => einsatztext = value; }

        public Einsatz()
        {
        }

        public override String ToString()
        {
            Type objType = this.GetType();
            PropertyInfo[] propertyInfoList = objType.GetProperties();
            StringBuilder result = new StringBuilder();
            result.Append("Einsatz (");
            foreach (PropertyInfo propertyInfo in propertyInfoList)
                result.AppendFormat("{0}={1} ", propertyInfo.Name, propertyInfo.GetValue(this));

            result.Append(")");

            return result.ToString();
        }

        public static Einsatz FromLeitstellenXml(XDocument xdocument)
        {
            IEnumerable<XElement> fields = xdocument.Descendants("Field").Where(field => field.Parent.Name == "Table" && field.Parent.Attribute("name").Value.ToUpper() == "EINSATZ");
            Einsatz einsatz = new Einsatz();
            foreach (XElement field in fields)
            {

                string fieldName = field.Attribute("name").Value;
                string fieldValue = field.Attribute("value").Value;

                System.Reflection.PropertyInfo propertyInfo = einsatz.GetType().GetProperty(fieldName.First().ToString().ToUpper() + fieldName.Substring(1).ToLower());
                propertyInfo.SetValue(einsatz, Convert.ChangeType(fieldValue, propertyInfo.PropertyType), null);
            }


            return einsatz;
        }
    }
}

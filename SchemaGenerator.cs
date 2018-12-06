using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

/**
 * Author: Pantelis Andrianakis
 * Date: November 15th 2017
 * Based on stackoverflow information: https://stackoverflow.com/questions/1065600/xsd-generator-from-several-xmls
 */
class SchemaGenerator
{
    static void Main(string[] args)
    {
        string xsdFile = "";
        string[] xmlFiles = null;
        DivideArguments(args, ref xsdFile, ref xmlFiles);

        if (FilesExist(xmlFiles))
        {
            Console.WriteLine("All files exist, good to proceed...");
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlSchemaInference inference = new XmlSchemaInference();

            bool bFirstTime = true;
            foreach (string sFile in xmlFiles)
            {
                XmlReader reader = XmlReader.Create(sFile);
                if (bFirstTime)
                {
                    schemaSet = inference.InferSchema(reader);
                }
                else
                {
                    schemaSet = inference.InferSchema(reader, schemaSet);
                }
                bFirstTime = false;
            }


            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t"
            };

            XmlWriter writer = XmlWriter.Create(xsdFile, xmlWriterSettings);

            foreach (XmlSchema schema in schemaSet.Schemas())
            {
                schema.Write(writer);
            }
            Console.WriteLine("Finished, wrote file to " + xsdFile + "...");
        }
    }

    static void DivideArguments(string[] args, ref string xsdFile, ref string[] xmlFiles)
    {
        xsdFile = args[0];
        xmlFiles = new string[args.Length - 1];

        for (int i = 0; i < args.Length - 1; i++)
        {
            xmlFiles[i] = args[i + 1];
        }
    }

    static bool FilesExist(string[] args)
    {
        bool bFilesExist = true;

        if (args.Length > 0)
        {
            foreach (string sFile in args)
            {
                if (!File.Exists(sFile))
                {
                    bFilesExist = false;
                }
            }
        }
        return bFilesExist;
    }
}
using UnityEngine;
using System.Reflection;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kawaii.ConfigSystem
{
    public interface ISGConfigDataTable
    {
        void LoadFromAssetPath(string path);

        void LoadFromAssetBundle(UnityEngine.Object textAsset, string path);

        void LoadFromString(string data);

        //#if UNITY_EDITOR
        void LoadFromFilePath(string path);
        //#endif

        bool HasData();
    }

    public class ConfigDataTable<TDataRecord> : ISGConfigDataTable where TDataRecord : class, new()
    {
        private readonly List<TDataRecord> _records = new List<TDataRecord>();
        public ReadOnlyCollection<TDataRecord> Records;
        private readonly Dictionary<string, object> _rebuildIndexDic = new Dictionary<string, object>();
        private readonly List<FieldInfo> _fields = new List<FieldInfo>();

        public class IndexField<TIndex> : Dictionary<TIndex, List<TDataRecord>>
        {

        };

        //public List<TDataRecord> Records
        //{
        //    get { return _records; }
        //}

        public ConfigDataTable()
        {
            Records = _records.AsReadOnly();
            Type type = typeof(TDataRecord);
            FieldInfo[] fieldArr = type.GetFields();
            foreach (FieldInfo filedInfo in fieldArr)
                if (!filedInfo.IsPrivate)
                    _fields.Add(filedInfo);
        }

        public bool HasData()
        {
            return _records.Count > 0;
        }

        public void LoadFromString(string data)
        {
            if (string.IsNullOrEmpty(data) || _fields == null || _fields.Count == 0)
                return;
            data = data.Replace("sep=\t\n", "");
            string[] lines = data.Split('\n');
            int line = 1;
            int length = lines.Length;

            string txt;
            while (line < length)
            {

                txt = lines[line++];
                if (string.IsNullOrEmpty(txt))
                    break;
                TDataRecord record = new TDataRecord();//Activator.CreateInstance<TDataRecord>();

                string[] columns = txt.Split('\t');

                if (columns == null || columns.Length < _fields.Count)
                {
                    Debug.LogError(txt);
                    Debug.LogError("Load config " + typeof(TDataRecord) + " line " + line + " error " + columns.Length + "," + _fields.Count);
                    continue;
                }

                int i = 0;
                bool error = false;

                foreach (FieldInfo field in _fields)
                {
                    object convert = ConvertData(columns[i], field.FieldType);
                    if (convert != null)
                        field.SetValue(record, convert);
                    else
                    {
                        error = true;
                        Debug.LogError("Load config " + typeof(TDataRecord) + " line " + line + " error text: \'" + columns[i] + "\'");
                        break;
                    }

                    i++;
                }
                if (error)
                {
                    //Debug.LogError ("Load config " + typeof(TDataRecord) + " line " + line + " error text:" + txt);
                    continue;
                }
                _records.Add(record);

            }

            RebuildIndex();
        }

        public void LoadFromAssetPath(string path)
        {
            if (_fields == null || _fields.Count == 0)
                return;

            FileInfo theSourceFile = null;
            TextReader reader = null;  // NOTE: TextReader, superclass of StreamReader and StringReader

            theSourceFile = new FileInfo(Application.dataPath + "/" + path + ".csv");
            if (theSourceFile != null && theSourceFile.Exists)
            {
                reader = theSourceFile.OpenText();  // returns StreamReader
            }
            else
            {
                TextAsset puzdata = (TextAsset)Resources.Load(path, typeof(TextAsset));
                reader = new StringReader(puzdata.text);  // returns StringReader
            }
            if (reader == null)
            {
                Debug.Log("not found or not readable");
            }
            else
            {
                int line = 0;
                string txt = reader.ReadLine();// bo dong dau
                if (txt.Contains("sep=\t"))
                    txt = reader.ReadLine();
                while (true)
                {
                    txt = reader.ReadLine();
                    line++;
                    if (string.IsNullOrEmpty(txt))
                        break;
                    TDataRecord record = new TDataRecord();//Activator.CreateInstance<TDataRecord>();

                    string[] columns = txt.Split('\t');

                    if (columns == null || columns.Length < _fields.Count)
                    {
                        Debug.LogError(txt);
                        Debug.LogError("Load config " + path + " line " + line + " error " + columns.Length + "," + _fields.Count);
                        continue;
                    }

                    int i = 0;
                    bool error = false;

                    foreach (FieldInfo field in _fields)
                    {
                        object convert = ConvertData(columns[i], field.FieldType);
                        if (convert != null)
                            field.SetValue(record, convert);
                        else
                        {
                            Debug.LogError(i + " | " + columns[i]);
                            error = true;
                            break;
                        }

                        i++;
                    }
                    if (error)
                    {
                        Debug.LogError("Load config " + path + " line " + line + " error text:" + txt);
                        continue;
                    }
                    _records.Add(record);

                }
                RebuildIndex();
            }
        }

        public void LoadFromAssetBundle(UnityEngine.Object textAsset, string path)
        {
            if (_fields == null || _fields.Count == 0)
                return;
            if (!textAsset)
                return;

            TextAsset text = textAsset as TextAsset;
            TextReader reader = new StringReader(text.text);

            // Debug.Log ("Load config " + path + " text=" + text);

            if (reader == null)
            {
                Debug.Log("not found or not readable");
            }
            else
            {
                int line = 0;
                string txt = reader.ReadLine();// bo dong dau
                if (txt.Contains("sep=\t"))
                    txt = reader.ReadLine();
                while (true)
                {
                    txt = reader.ReadLine();
                    line++;
                    if (string.IsNullOrEmpty(txt))
                        break;
                    TDataRecord record = new TDataRecord();//Activator.CreateInstance<TDataRecord>();

                    string[] columns = txt.Split('\t');

                    if (columns == null || columns.Length < _fields.Count)
                    {
                        Debug.LogError(txt);
                        Debug.LogError("Load config " + path + " line " + line + " error " + columns.Length + "," + _fields.Count);
                        continue;
                    }

                    int i = 0;
                    bool error = false;

                    foreach (FieldInfo field in _fields)
                    {
                        object convert = ConvertData(columns[i], field.FieldType);
                        if (convert != null)
                            field.SetValue(record, convert);
                        else
                        {
                            Debug.LogError(columns[i]);
                            error = true;
                            break;
                        }

                        i++;
                    }
                    if (error)
                    {
                        Debug.LogError("Load config " + path + " line " + line + " error text:" + txt);
                        continue;
                    }
                    _records.Add(record);

                }
                RebuildIndex();
            }
        }
        //#if UNITY_EDITOR
        public void LoadFromFilePath(string path)
        {
            if (_fields == null || _fields.Count == 0)
                return;

            using (StreamReader sr = new StreamReader(path))
            {
                string txt = sr.ReadLine();// bo dong dau
                if (txt.Contains("sep=\t"))
                    txt = sr.ReadLine();
                int line = 0;
                while ((txt = sr.ReadLine()) != null)
                {
                    line++;
                    if (string.IsNullOrEmpty(txt))
                        break;
                    TDataRecord record = new TDataRecord();//Activator.CreateInstance<TDataRecord>();

                    string[] columns = txt.Split('\t');

                    if (columns == null || columns.Length < _fields.Count)
                    {
                        Debug.LogError(txt);
                        Debug.LogError("Load config " + path + " line " + line + " error " + columns.Length + "," + _fields.Count);
                        continue;
                    }

                    int i = 0;
                    bool error = false;

                    foreach (FieldInfo field in _fields)
                    {
                        object convert = ConvertData(columns[i], field.FieldType);
                        if (convert != null)
                            field.SetValue(record, convert);
                        else
                        {
                            Debug.LogError(columns[i]);
                            error = true;
                            break;
                        }

                        i++;
                    }
                    if (error)
                    {
                        Debug.LogError("Load config " + path + " line " + line + " error text:" + txt);
                        continue;
                    }
                    _records.Add(record);

                }
                RebuildIndex();

            }
        }

        //#endif

        void Clear()
        {
            _records.Clear();
            _rebuildIndexDic.Clear();
        }

        protected virtual void RebuildIndex()
        {

        }

        protected void RebuildIndexByField<TIndex>(string fieldName, bool canMulti = false)
        {
            Type recordType = typeof(TDataRecord);
            FieldInfo fieldInfo = recordType.GetField(fieldName);
            if (fieldInfo == null)
                throw new Exception("Field [" + fieldName + "] not found");
            IndexField<TIndex> indexField = new IndexField<TIndex>();
            _rebuildIndexDic[fieldName] = indexField;
            foreach (TDataRecord record in _records)
            {
                var fieldValue = (TIndex)fieldInfo.GetValue(record);

                List<TDataRecord> indexedValue;
                if (!indexField.TryGetValue(fieldValue, out indexedValue))
                {
                    indexedValue = new List<TDataRecord>();
                    indexField[fieldValue] = indexedValue;
                }
                else if (!canMulti)
                {
                    Debug.LogError("Trung id: " + typeof(TDataRecord) + "," + fieldValue);
                }
                indexedValue.Add(record);

            }
        }

        public TDataRecord GetRecordByIndex<TIndex>(string fieldName, TIndex compareValue)
        {
            object dic = null;
            if (_rebuildIndexDic.TryGetValue(fieldName, out dic))
            {
                IndexField<TIndex> indexField = (IndexField<TIndex>)dic;    //???indexField receive data by dictionary type from dic
                List<TDataRecord> resultList = null;
                if (indexField.TryGetValue(compareValue, out resultList))   //get value with Id compareValue (int) to resultList: ConfigRoomRecord
                    if (resultList.Count > 0)
                        return resultList[0];

                return null;
            }
            return null;
        }

        public List<TDataRecord> GetRecordsByIndex<TIndex>(string fieldName, TIndex compareValue)
        {
            object dic = null;
            if (_rebuildIndexDic.TryGetValue(fieldName, out dic))
            {
                IndexField<TIndex> indexField = (IndexField<TIndex>)dic;
                List<TDataRecord> resultList = null;
                if (indexField.TryGetValue(compareValue, out resultList))
                    if (resultList.Count > 0)
                        return resultList;

                return null;
            }
            return null;
        }

        public IndexField<TIndex> GetIndexField<TIndex>(string fieldName)
        {
            object dic = null;
            if (_rebuildIndexDic.TryGetValue(fieldName, out dic))
            {
                return (IndexField<TIndex>)dic;
            }
            return null;
        }

        object ConvertData(string value, Type t)
        {
            bool isEnum = false;
#if UNITY_WINRT && !UNITY_EDITOR
        isEnum = t.GetTypeInfo().IsEnum; 
#else
            isEnum = t.IsEnum;
#endif
            if (isEnum)
            {
                Array arr = Enum.GetValues(t);
                if (string.IsNullOrEmpty(value))
                    return arr.GetValue(0);
                foreach (object item in arr)
                {
                    if (item.ToString().ToLower().Equals(value.Trim().ToLower()))
                        return item;
                }
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(t); ; //t.AsTypeCode(); 
                if (typeCode == TypeCode.Int32)
                {
                    int result;
                    if (int.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.Int64)
                {
                    long result;
                    if (long.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.Single || typeCode == TypeCode.Decimal)
                {
                    float result;
                    if (float.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.String)
                    return value;
                else if (typeCode == TypeCode.Boolean)
                {
                    bool result;
                    if (bool.TryParse(value, out result))
                        return result;
                    if (value == "0")
                        return false;
                    else if (value == "1")
                        return true;
                }
                else if (typeCode == TypeCode.Double)
                {
                    double result;
                    if (double.TryParse(value, out result))
                        return result;
                    return null;
                }
                else if (typeCode == TypeCode.DateTime)
                {
                    DateTime result;
                    if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss,fff",
                            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                        return result;
                    return null;
                }
            }
            return null;
        }

    }
}

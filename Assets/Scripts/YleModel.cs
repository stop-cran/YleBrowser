using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class Meta
    {
        public string offset;
        public string limit;
        public int count;
        public int program;
        public int clip;
        public string q;
        public string language;
    }

    [Serializable]
    public class Description
    {
        public string fi;
    }

    [Serializable]
    public class Subject
    {
        public string id;
        public Title title;
    }

    [Serializable]
    public class PartOfSeries
    {
        public string id;
        public Description description;
        public Title title;
        public List<Subject> subject = new List<Subject>();
        public Description availabilityDescription;
    }

    [Serializable]
    public class Audio
    {
        public List<string> language = new List<string>();
        public string type;
    }

    [Serializable]
    public class PublicationEvent
    {
        public string id;
        public int version;
        public string startTime;
        public string endTime;
    }

    [Serializable]
    public class Title
    {
        public string fi;
    }

    [Serializable]
    public class Notation
    {
        public string value;
        public string valueType;
    }

    [Serializable]
    public class Datum
    {
        public Title title;
        public List<Subject> subject = new List<Subject>();
        public List<PublicationEvent> publicationEvent = new List<PublicationEvent>();
        public List<Audio> audio = new List<Audio>();
        public PartOfSeries partOfSeries;
    }

    [Serializable]
    public class YleModel
    {
        public string apiVersion;
        public Meta meta;
        public List<Datum> data = new List<Datum>();
    }
}

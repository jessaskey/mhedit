using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace GameEditor.Core.Xml
{

    /// <remarks />
    [ GeneratedCode( "xsd", "4.6.1055.0" ) ]
    [ Serializable ]
    [ DebuggerStepThrough ]
    [ DesignerCategory( "code" ) ]
    //[ XmlType( AnonymousType = true, Namespace = "http://askey.org/GameProfile" ) ]
    [ XmlRoot( Namespace = "http://askey.org/GameProfile", IsNullable = false ) ]
    public class GameProfile
    {
        private List<FeatureGroup> _featureGroupsField;

        private List<Feature> _featuresField;

        private GameInfo _gameInfoField;

        private Levels _levelsField;

        private MemoryMap _memoryMapField;

        private ProgramMemory _programMemoryField;

        private string _versionField;

        /// <remarks />
        public GameInfo GameInfo
        {
            get { return this._gameInfoField; }
            set { this._gameInfoField = value; }
        }

        /// <remarks />
        public MemoryMap MemoryMap
        {
            get { return this._memoryMapField; }
            set { this._memoryMapField = value; }
        }

        /// <remarks />
        public ProgramMemory ProgramMemory
        {
            get { return this._programMemoryField; }
            set { this._programMemoryField = value; }
        }

        /// <remarks />
        [ XmlArrayItem( "Feature", IsNullable = false ) ]
        public List<Feature> Features
        {
            get { return this._featuresField; }
            set { this._featuresField = value; }
        }

        /// <remarks />
        [ XmlArrayItem( "FeatureGroup", IsNullable = false ) ]
        public List<FeatureGroup> FeatureGroups
        {
            get { return this._featureGroupsField; }
            set { this._featureGroupsField = value; }
        }

        /// <remarks />
        public Levels Levels
        {
            get { return this._levelsField; }
            set { this._levelsField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Version
        {
            get { return this._versionField; }
            set { this._versionField = value; }
        }
    }

}
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
        private List<FeatureGroup> _featureGroups;

        private List<Feature> _features;

        private GameInfo _gameInfo;

        private Levels _levels;

        private HardwareDescription _hardwareDescription;

        private ProgramMemory _programMemory;

        private string _version;

        /// <remarks />
        public GameInfo GameInfo
        {
            get { return this._gameInfo; }
            set { this._gameInfo = value; }
        }

        /// <remarks />
        public HardwareDescription HardwareDescription
        {
            get { return this._hardwareDescription; }
            set { this._hardwareDescription = value; }
        }

        /// <remarks />
        public ProgramMemory ProgramMemory
        {
            get { return this._programMemory; }
            set { this._programMemory = value; }
        }

        /// <remarks />
        [ XmlArrayItem( "Feature", IsNullable = false ) ]
        public List<Feature> Features
        {
            get { return this._features; }
            set { this._features = value; }
        }

        /// <remarks />
        [ XmlArrayItem( "FeatureGroup", IsNullable = false ) ]
        public List<FeatureGroup> FeatureGroups
        {
            get { return this._featureGroups; }
            set { this._featureGroups = value; }
        }

        /// <remarks />
        public Levels Levels
        {
            get { return this._levels; }
            set { this._levels = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Version
        {
            get { return this._version; }
            set { this._version = value; }
        }
    }

}
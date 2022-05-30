using System;
using mhedit.Containers.Validation;
using MajorHavocEditor.Interfaces.Ui;
using MajorHavocEditor.Views;

namespace MajorHavocEditor.Services
{

    public interface IValidationService
    {
        IValidationResult Validate( object subject );

        void ValidateAndDisplayResults( object subject, string windowTitle = "" );
    }

    public class ValidationService : IValidationService
    {
        private readonly IWindowManager _windowManager;

        private readonly ValidationWindow _validationWindow = new Lazy<ValidationWindow>(
            () => new ValidationWindow() ).Value;

        public ValidationService( IWindowManager windowManager )
        {
            this._windowManager = windowManager;
        }

#region Implementation of IValidationService

        /// <inheritdoc />
        public IValidationResult Validate( object subject )
        {
            return subject.Validate();
        }

        public void ValidateAndDisplayResults( object subject, string windowTitle = "" )
        {
            IValidationResult validationResult = subject.Validate();

            this._validationWindow.Add( new ValidationResultTab( validationResult ) );

            this._windowManager.Show( this._validationWindow );
        }

#endregion

    }

}

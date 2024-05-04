// ReSharper disable InconsistentNaming

namespace NetStd.Prism.July2018
{
    public static class Resources
    {
        public const string CannotRegisterCompositeCommandInItself = "Cannot register a CompositeCommand in itself.";

        public const string CannotRegisterSameCommandTwice =
            "Cannot register the same command twice in the same CompositeCommand.";

        public const string DelegateCommandDelegatesCannotBeNull =
            "Neither the executeMethod nor the canExecuteMethod delegates can be null.";

        public const string DelegateCommandInvalidGenericPayloadType =
            "T for DelegateCommand<T>; is not an object nor Nullable.";

        public const string EventAggregatorNotConstructedOnUIThread =
            "To use the UIThread option for subscribing, the EventAggregator must be constructed on the UI thread.";

        public const string InvalidDelegateRerefenceTypeException = "Invalid Delegate Reference Type Exception";
        public const string InvalidPropertyNameException = "The entity does not contain a property with that name";

        public const string PropertySupport_ExpressionNotProperty_Exception =
            "The member access expression does not access a property.";

        public const string PropertySupport_NotMemberAccessExpression_Exception =
            "The expression is not a member access expression.";

        public const string PropertySupport_StaticExpression_Exception =
            "The referenced property is a static property.";
    }
}
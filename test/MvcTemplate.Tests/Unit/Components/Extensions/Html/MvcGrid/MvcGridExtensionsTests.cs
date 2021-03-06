﻿using MvcTemplate.Components.Extensions.Html;
using MvcTemplate.Components.Security;
using MvcTemplate.Resources;
using MvcTemplate.Tests.Objects;
using NonFactors.Mvc.Grid;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace MvcTemplate.Tests.Unit.Components.Extensions.Html
{
    [TestFixture]
    public class MvcGridExtensionsTests
    {
        private IGridColumns<AllTypesView> columns;
        private IGridColumn<AllTypesView> column;
        private IHtmlGrid<AllTypesView> htmlGrid;
        private UrlHelper urlHelper;

        [SetUp]
        public void SetUp()
        {
            column = SubstituteColumn<AllTypesView>();
            htmlGrid = SubstituteHtmlGrid<AllTypesView>();
            columns = SubstituteColumns<AllTypesView, DateTime?>(column);

            HttpContext.Current = HttpContextFactory.CreateHttpContext();
            urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        [TearDown]
        public void TearDown()
        {
            Authorization.Provider = null;
            HttpContext.Current = null;
        }

        #region Extension method: AddActionLink<T>(this IGridColumns<T> columns, String action, String iconClass)

        [Test]
        public void AddActionLink_OnUnauthorizedActionLinkReturnsNull()
        {
            Authorization.Provider = Substitute.For<IAuthorizationProvider>();

            Assert.IsNull(columns.AddActionLink("Edit", "fa fa-pencil"));
        }

        [Test]
        public void AddActionLink_RendersAuthorizedActionLink()
        {
            String actionLink = "";
            AllTypesView view = new AllTypesView();
            Authorization.Provider = Substitute.For<IAuthorizationProvider>();
            Authorization.Provider.IsAuthorizedFor(Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>(), "Details").Returns(true);

            columns
                .Add(Arg.Any<Expression<Func<AllTypesView, String>>>())
                .Returns(column)
                .AndDoes(info =>
                {
                    actionLink = info.Arg<Expression<Func<AllTypesView, String>>>().Compile().Invoke(view);
                });

            columns.AddActionLink("Details", "fa fa-info");

            String actual = actionLink;
            String expected = String.Format(
                "<a class=\"details-action\" href=\"{0}\">" +
                    "<i class=\"fa fa-info\"></i>" +
                "</a>",
                urlHelper.Action("Details", new { id = view.Id }));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddActionLink_OnNullAuthorizationProviderRendersActionLink()
        {
            AllTypesView view = new AllTypesView();
            Authorization.Provider = null;
            String actionLink = "";

            columns
                .Add(Arg.Any<Expression<Func<AllTypesView, String>>>()).Returns(column)
                .AndDoes(info => { actionLink = info.Arg<Expression<Func<AllTypesView, String>>>().Compile().Invoke(view); });

            columns.AddActionLink("Details", "fa fa-info");

            String actual = actionLink;
            String expected = String.Format(
                "<a class=\"details-action\" href=\"{0}\">" +
                    "<i class=\"fa fa-info\"></i>" +
                "</a>",
                urlHelper.Action("Details", new { id = view.Id }));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddActionLink_OnModelWihoutKeyPropertyThrows()
        {
            Func<Object, String> renderer = null;
            IGridColumn<Object> column = SubstituteColumn<Object>();
            IGridColumns<Object> columns = SubstituteColumns<Object, String>(column);

            columns
                .Add(Arg.Any<Expression<Func<Object, String>>>())
                .Returns(column)
                .AndDoes(info =>
                {
                    renderer = info.Arg<Expression<Func<Object, String>>>().Compile();
                });

            columns.AddActionLink("Delete", "fa fa-times");

            String actual = Assert.Throws<Exception>(() => renderer.Invoke(new Object())).Message;
            String expected = "Object type does not have a key property.";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddActionLink_SetsCssOnGridColumn()
        {
            columns.AddActionLink("Edit", "fa fa-pencil");

            column.Received().Css("action-cell");
        }

        [Test]
        public void AddActionLink_DoesNotEncodeGridColumn()
        {
            columns.AddActionLink("Edit", "fa fa-pencil");

            column.Received().Encoded(false);
        }

        #endregion

        #region Extension method: AddDateProperty<T>(this IGridColumns<T> columns, Expression<Func<T, DateTime>> property)

        [Test]
        public void AddDateProperty_AddsGridColumn()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.DateTimeField;

            columns.AddDateProperty(expression);

            columns.Received().Add(expression);
        }

        [Test]
        public void AddDateProperty_SetsGridColumnTitle()
        {
            String title = ResourceProvider.GetPropertyTitle<AllTypesView, DateTime?>(model => model.DateTimeField);

            columns.AddDateProperty(model => model.DateTimeField);

            column.Received().Titled(title);
        }

        [Test]
        public void AddDateProperty_SetsGridColumnCss()
        {
            columns.AddDateProperty(model => model.DateTimeField);

            column.Received().Css("date-cell");
        }

        [Test]
        public void AddDateProperty_FormatsGridColumn()
        {
            columns.AddDateProperty(model => model.DateTimeField);

            column.Received().Formatted("{0:d}");
        }

        #endregion

        #region Extension method: AddDateProperty<T>(this IGridColumns<T> columns, Expression<Func<T, DateTime?>> property)

        [Test]
        public void AddDateProperty_Nullable_AddsGridColumn()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            columns.AddDateProperty(expression);

            columns.Received().Add(expression);
        }

        [Test]
        public void AddDateProperty_Nullable_SetsGridColumnTitle()
        {
            String title = ResourceProvider.GetPropertyTitle<AllTypesView, DateTime?>(model => model.NullableDateTimeField);

            columns.AddDateProperty(model => model.NullableDateTimeField);

            column.Received().Titled(title);
        }

        [Test]
        public void AddDateProperty_Nullable_SetsGridColumnCss()
        {
            columns.AddDateProperty(model => model.NullableDateTimeField);

            column.Received().Css("date-cell");
        }

        [Test]
        public void AddDateProperty_Nullable_FormatsGridColumn()
        {
            columns.AddDateProperty(model => model.NullableDateTimeField);

            column.Received().Formatted("{0:d}");
        }

        #endregion

        #region Extension method: AddDateTimeProperty<T>(this IGridColumns<T> columns, Expression<Func<T, DateTime>> property)

        [Test]
        public void AddDateTimeProperty_AddsGridColumn()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.DateTimeField;

            columns.AddDateTimeProperty(expression);

            columns.Received().Add(expression);
        }

        [Test]
        public void AddDateTimeProperty_SetsGridColumnTitle()
        {
            String title = ResourceProvider.GetPropertyTitle<AllTypesView, DateTime?>(model => model.DateTimeField);

            columns.AddDateTimeProperty(model => model.DateTimeField);

            column.Received().Titled(title);
        }

        [Test]
        public void AddDateTimeProperty_SetsGridColumnCss()
        {
            columns.AddDateTimeProperty(model => model.DateTimeField);

            column.Received().Css("date-cell");
        }

        [Test]
        public void AddDateTimeProperty_FormatsGridColumn()
        {
            columns.AddDateTimeProperty(model => model.DateTimeField);

            column.Received().Formatted("{0:g}");
        }

        #endregion

        #region Extension method: AddDateTimeProperty<T>(this IGridColumns<T> columns, Expression<Func<T, DateTime?>> property)

        [Test]
        public void AddDateTimeProperty_Nullable_AddsGridColumn()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            columns.AddDateTimeProperty(expression);

            columns.Received().Add(expression);
        }

        [Test]
        public void AddDateTimeProperty_Nullable_SetsGridColumnTitle()
        {
            String title = ResourceProvider.GetPropertyTitle<AllTypesView, DateTime?>(model => model.NullableDateTimeField);

            columns.AddDateTimeProperty(model => model.NullableDateTimeField);

            column.Received().Titled(title);
        }

        [Test]
        public void AddDateTimeProperty_Nullable_SetsGridColumnCss()
        {
            columns.AddDateTimeProperty(model => model.NullableDateTimeField);

            column.Received().Css("date-cell");
        }

        [Test]
        public void AddDateTimeProperty_Nullable_FormatsGridColumn()
        {
            columns.AddDateTimeProperty(model => model.NullableDateTimeField);

            column.Received().Formatted("{0:g}");
        }

        #endregion

        #region Extension method: AddProperty<T, TProperty>(this IGridColumns<T> columns, Expression<Func<T, TProperty>> property)

        [Test]
        public void AddProperty_AddsGridColumn()
        {
            Expression<Func<AllTypesView, String>> expression = (model) => model.Id;

            columns.AddProperty(expression);

            columns.Received().Add(expression);
        }

        [Test]
        public void AddProperty_SetsGridColumnTitle()
        {
            String title = ResourceProvider.GetPropertyTitle<AllTypesView, DateTime?>(model => model.NullableDateTimeField);

            columns.AddProperty(model => model.NullableDateTimeField);

            column.Received().Titled(title);
        }

        [Test]
        public void AddProperty_SetsCssClassAsTextCellForEnum()
        {
            AssertCssClassFor(model => model.EnumField, "text-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForSByte()
        {
            AssertCssClassFor(model => model.SByteField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForByte()
        {
            AssertCssClassFor(model => model.ByteField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForInt16()
        {
            AssertCssClassFor(model => model.Int16Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForUInt16()
        {
            AssertCssClassFor(model => model.UInt16Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForInt32()
        {
            AssertCssClassFor(model => model.Int32Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForUInt32()
        {
            AssertCssClassFor(model => model.UInt32Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForInt64()
        {
            AssertCssClassFor(model => model.Int64Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForUInt64()
        {
            AssertCssClassFor(model => model.UInt64Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForSingle()
        {
            AssertCssClassFor(model => model.SingleField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForDouble()
        {
            AssertCssClassFor(model => model.DoubleField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForDecimal()
        {
            AssertCssClassFor(model => model.DecimalField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsDateCellForDateTime()
        {
            AssertCssClassFor(model => model.DateTimeField, "date-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsTextCellForNullableEnum()
        {
            AssertCssClassFor(model => model.NullableEnumField, "text-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableSByte()
        {
            AssertCssClassFor(model => model.NullableSByteField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableByte()
        {
            AssertCssClassFor(model => model.NullableByteField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableInt16()
        {
            AssertCssClassFor(model => model.NullableInt16Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableUInt16()
        {
            AssertCssClassFor(model => model.NullableUInt16Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableInt32()
        {
            AssertCssClassFor(model => model.NullableInt32Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableUInt32()
        {
            AssertCssClassFor(model => model.NullableUInt32Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableInt64()
        {
            AssertCssClassFor(model => model.NullableInt64Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableUInt64()
        {
            AssertCssClassFor(model => model.NullableUInt64Field, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableSingle()
        {
            AssertCssClassFor(model => model.NullableSingleField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableDouble()
        {
            AssertCssClassFor(model => model.NullableDoubleField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsNumberCellForNullableDecimal()
        {
            AssertCssClassFor(model => model.NullableDecimalField, "number-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsDateCellForNullableDateTime()
        {
            AssertCssClassFor(model => model.NullableDateTimeField, "date-cell");
        }

        [Test]
        public void AddProperty_SetsCssClassAsTextCellForOtherTypes()
        {
            AssertCssClassFor(model => model.StringField, "text-cell");
        }

        #endregion

        #region Extension method: ApplyDefaults<T>(this IHtmlGrid<T> grid)

        [Test]
        public void ApplyDefaults_SetsEmptyText()
        {
            htmlGrid.ApplyDefaults();

            htmlGrid.Received().Empty(MvcTemplate.Resources.Table.Resources.NoDataFound);
        }

        [Test]
        public void ApplyDefaults_SetsNameByReplacingViewToEmpty()
        {
            htmlGrid.ApplyDefaults();

            htmlGrid.Received().Named("AllTypes");
        }

        [Test]
        public void ApplyDefaults_SetsCss()
        {
            htmlGrid.ApplyDefaults();

            htmlGrid.Received().Css("table-hover");
        }

        [Test]
        public void ApplyDefaults_EnablesFiltering()
        {
            htmlGrid.ApplyDefaults();

            htmlGrid.Received().Filterable();
        }

        [Test]
        public void ApplyDefaults_EnablesSorting()
        {
            htmlGrid.ApplyDefaults();

            htmlGrid.Received().Sortable();
        }

        [Test]
        public void ApplyDefaults_EnablesPaging()
        {
            htmlGrid.ApplyDefaults();

            htmlGrid.Received().Pageable();
        }

        #endregion

        #region Test helpers

        private IHtmlGrid<TModel> SubstituteHtmlGrid<TModel>()
        {
            IHtmlGrid<TModel> grid = Substitute.For<IHtmlGrid<TModel>>();
            grid.Empty(Arg.Any<String>()).Returns(grid);
            grid.Named(Arg.Any<String>()).Returns(grid);
            grid.Css(Arg.Any<String>()).Returns(grid);
            grid.Filterable().Returns(grid);
            grid.Sortable().Returns(grid);
            grid.Pageable().Returns(grid);

            return grid;
        }
        private IGridColumn<TModel> SubstituteColumn<TModel>()
        {
            IGridColumn<TModel> column = Substitute.For<IGridColumn<TModel>>();
            column.Formatted(Arg.Any<String>()).Returns(column);
            column.Encoded(Arg.Any<Boolean>()).Returns(column);
            column.Titled(Arg.Any<String>()).Returns(column);
            column.Css(Arg.Any<String>()).Returns(column);

            return column;
        }
        private IGridColumns<TModel> SubstituteColumns<TModel, TProperty>(IGridColumn<TModel> column)
        {
            IGridColumns<TModel> columns = Substitute.For<IGridColumns<TModel>>();
            columns.Add(Arg.Any<Expression<Func<TModel, String>>>()).Returns(column);
            columns.Add(Arg.Any<Expression<Func<TModel, DateTime>>>()).Returns(column);
            columns.Add(Arg.Any<Expression<Func<TModel, DateTime?>>>()).Returns(column);
            columns.Add(Arg.Any<Expression<Func<TModel, TProperty>>>()).Returns(column);

            return columns;
        }

        private void AssertCssClassFor<TProperty>(Expression<Func<AllTypesView, TProperty>> property, String expected)
        {
            columns = SubstituteColumns<AllTypesView, TProperty>(column);
            columns.AddProperty(property);

            column.Received().Css(expected);
        }

        #endregion
    }
}

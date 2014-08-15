using System;
using System.Collections.Generic;
using System.Globalization;

namespace Sprint.Grid.Examples.Helpers
{
    public static class GridExtensions
    {

        private static readonly IDictionary<string, Action<IGridModel>> LocalizationActions =
            new Dictionary<string, Action<IGridModel>>();

        static GridExtensions()
        {
            LocalizationActions["ru"] = model => {

                model.GridRenderOptions.EmptyGroupHeaderText = "Перетащите сюда заголовок столбца, для группировки";
                model.GridRenderOptions.EmptyText = "Нет данных для отображения";
                model.GridRenderOptions.GroupTagTitle = "Группировка";

                model.PagedListRenderOptions.PageFormat = "Страница {0} из {1}";
                model.PagedListRenderOptions.TotalFormat = "Показаны {0} - {1} из {2}";
                model.PagedListRenderOptions.TotalSingleFormat = "Показаны {0} из {1}";
            };

            LocalizationActions["fr"] = model => {

                model.GridRenderOptions.EmptyGroupHeaderText = "Faites glisser un en-tête de colonne et déposez ici pour groupe par cette colonne";
                model.GridRenderOptions.EmptyText = "Vide";
                model.GridRenderOptions.EmptyGroupTitleText = "[Vide]";
                model.GridRenderOptions.GroupTagTitle = "Groupement";

                model.PagedListRenderOptions.PageFormat = "Page {0} de {1}";
                model.PagedListRenderOptions.TotalFormat = "Démontrant {0} - {1} de {2}";
                model.PagedListRenderOptions.TotalSingleFormat = "Démontrant {0} de {1}";
            };

            //add other languages
        }

        public static IGridModel<TModel> Localize<TModel>(this IGridModel<TModel> model, CultureInfo culture = null) where TModel : class
        {
            if(model == null)
                return null;

            culture = culture ?? CultureInfo.CurrentUICulture;

            var action = LocalizationActions[culture.TwoLetterISOLanguageName];

            if(action != null)
                action(model);

            return model;
        }
    }
}
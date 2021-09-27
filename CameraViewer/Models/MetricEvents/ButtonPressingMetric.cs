using System;

namespace CameraViewer.Models.MetricEvents
{
    /// <summary>
    /// Событие метрики нажатия кнопки
    /// </summary>
    public class ButtonPressingMetric
    {
        /// <summary>
        /// Период (без времени)
        ///</summary>
        public DateTime SysRowDate { get; set; }
        
        /// <summary>
        /// Guid сессии
        ///</summary>
        public Guid SessionId { get; set; }
        
        /// <summary>
        /// Дата и время события по UTC
        ///</summary>
        public DateTime EventDateUTC { get; set; }
        
        /// <summary>
        /// Часовая зона
        ///</summary>
        public int UserTimeZone { get; set; }
        
        /// <summary>
        /// Идентификатор кнопки (Мак/зона/код филиала)
        ///</summary>
        public string ButtonId { get; set; }
        
        /// <summary>
        /// Наименование кнопки
        ///</summary>
        public string ButtonName { get; set; }
        
        /// <summary>
        /// Заголовок события
        ///</summary>
        public string EventTitle { get; set; }
        
        /// <summary>
        /// Описание события
        ///</summary>
        public string EventDescription { get; set; }
        
        /// <summary>
        /// Идентификатор Дивизиона
        ///</summary>
        public Guid DivisionId { get; set; }
        
        /// <summary>
        /// Наименование Дивизиона
        ///</summary>
        public string DivisionName { get; set; }
        
        /// <summary>
        /// Идентификатор РРС
        ///</summary>
        public Guid RDCId { get; set; }
        
        /// <summary>
        /// Наименование РРС
        ///</summary>
        public string RDCName { get; set; }
        
        /// <summary>
        /// Идентификатор филиала
        ///</summary>
        public Guid BranchId { get; set; }
        
        /// <summary>
        /// Название филиала
        ///</summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sysRowDate">Период (без времени)</param>
        /// <param name="sessionId">Идентификатор сессии</param>
        /// <param name="eventDateUtc">Дата и время события по UTC</param>
        /// <param name="userTimeZone">Часовая зона</param>
        /// <param name="buttonId">Идентификатор кнопки (Мак/зона/код филиала)</param>
        /// <param name="buttonName">Наименование кнопки</param>
        /// <param name="eventTitle">Заголовок события</param>
        /// <param name="eventDescription">Описание события</param>
        /// <param name="divisionId">Идентификатор Дивизиона</param>
        /// <param name="divisionName">Наименование Дивизиона</param>
        /// <param name="rdcId">Идентификатор РРС</param>
        /// <param name="rdcName">Наименование РРС</param>
        /// <param name="branchId">Идентификатор филиала</param>
        /// <param name="branchName">Название филиала</param>
        public ButtonPressingMetric(DateTime sysRowDate, Guid sessionId, DateTime eventDateUtc, int userTimeZone, 
            string buttonId, string buttonName, string eventTitle, string eventDescription, Guid divisionId,
            string divisionName, Guid rdcId, string rdcName, Guid branchId, string branchName)
        {
            SysRowDate = sysRowDate;
            SessionId = sessionId;
            EventDateUTC = eventDateUtc;
            UserTimeZone = userTimeZone;
            ButtonId = buttonId;
            ButtonName = buttonName;
            EventTitle = eventTitle;
            EventDescription = eventDescription;
            DivisionId = divisionId;
            DivisionName = divisionName;
            RDCId = rdcId;
            RDCName = rdcName;
            BranchId = branchId;
            BranchName = branchName;
        }
    }
}
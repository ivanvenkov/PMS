const getDate = (date: string | Date) => {
  const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  const months = [
    'Jan',
    'Feb',
    'Mar',
    'Apr',
    'May',
    'Jun',
    'Jul',
    'Aug',
    'Sept',
    'Oct',
    'Nov',
    'Dec'
  ];

  const startDate = new Date(date);

  return `${weekDays[startDate.getDay()]}, ${months[startDate.getMonth()]} ${startDate.getDate()}
   ${startDate.getHours()}:${startDate.getMinutes()}h`;
};

export default getDate;

import axios from 'axios';
// כתובת url כdefult
axios.defaults.baseURL = 'http://localhost:5000';
// כתובת
const apiUrl = "http://localhost:5165"; 

axios.interceptors.response.use(
  response => response,
  error => {
      console.log("Error Data:", error.response?.data);
  }
);
// שליפת כל המשימות
export default {
  getTasks: async () => {
    const result = await axios.get(`${apiUrl}`); // ודא שה-URL נכון
    return result.data;
  },
  // הוספת משימה
addTask: async(name) => {
  console.log('addTask', name);
  await axios.post(`${apiUrl}/addItem`, name, {
      headers: {
          'Content-Type': 'application/json' 
      }
  });
},
// עדכון משימה
setCompleted: async(id, isComplete)=>{
  console.log('setCompleted', {id, isComplete})
  await axios.put(`${apiUrl}/updateItem?id=${id}&&IsComplete=${isComplete}`);
},
  // מחיקת משימה לפי הid
  deleteTask: async(idTask) => {
    console.log('deleteTask', { idTask });
    const result = await axios.delete(`${apiUrl}/delete/${idTask}`);    
    return result.data;
  }
};


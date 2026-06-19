import { createContext, useContext, useState } from "react";

const NotificationContext = createContext();

export function useNotify() {
  return useContext(NotificationContext);
}

export function NotificationProvider({ children }) {
  const [notifications, setNotifications] = useState([]);

  function notify(type, message) {
    const id = Date.now();
    setNotifications(n => [...n, { id, type, message }]);

    setTimeout(() => {
      setNotifications(n => n.filter(x => x.id !== id));
    }, 5000);
  }

  return (
    <NotificationContext.Provider value={{ notify }}>
      {children}
      <div className="toastContainer">
        {notifications.map(n => (
          <div key={n.id} className={`toast ${n.type}`}>
            {n.message}
          </div>
        ))}
      </div>
    </NotificationContext.Provider>
  );
}

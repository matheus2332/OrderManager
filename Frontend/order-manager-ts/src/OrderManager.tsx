import React, { useState, useEffect } from "react";

const API_BASE = "http://localhost:5009";

interface Order {
  id: number;
  quantity: number;
  completed: boolean;
  createdAt: string;
}

interface InventoryResponse {
  available: number;
  reserved: number;
}

interface CreateOrderResponse {
  success: boolean;
  errorMessage?: string;
}

export default function OrderManager(): JSX.Element {
  const [orders, setOrders] = useState<Order[]>([]);
  const [quantity, setQuantity] = useState<number>(1);
  const [stock, setStock] = useState<number>(0);
  const [reserved, setReserved] = useState<number>(0);
  const [error, setError] = useState<string>("");

  const fetchOrders = async () => {
    try {
      const res = await fetch(`${API_BASE}/orders`);
      if (!res.ok) throw new Error("Failed to fetch orders");
      const data: Order[] = await res.json();
      setOrders(data);
    } catch (err) {
      console.error(err);
      setError("Failed to fetch orders");
    }
  };

  const fetchStock = async () => {
    try {
      const res = await fetch(`${API_BASE}/inventory`);
      if (!res.ok) throw new Error("Failed to fetch stock");
      const data: InventoryResponse = await res.json();
      setStock(data.available);
      setReserved(data.reserved);
    } catch (err) {
      console.error(err);
      setError("Failed to fetch stock");
    }
  };

  useEffect(() => {
    fetchOrders();
    fetchStock();

    const intervalId = setInterval(() => {
      fetchOrders();
      fetchStock();
    }, 5000);

    return () => clearInterval(intervalId);
  }, []);

  const handleCreateOrder = async () => {
    setError("");
    if (quantity <= 0) {
      setError("Quantity must be greater than zero");
      return;
    }

    try {
      const res = await fetch(`${API_BASE}/orders`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ quantity }),
      });
      const data: CreateOrderResponse = await res.json();

      if (res.ok) {
        if (data.success) {
          setQuantity(1);
          await fetchOrders();
          await fetchStock();
        } else {
          setError(data.errorMessage || "Unknown error");
        }
      } else {
        setError("Failed to create order");
      }
    } catch {
      setError("Failed to create order");
    }
  };

  const handleCompleteOrder = async (id: number) => {
    try {
      const res = await fetch(`${API_BASE}/orders/${id}/complete`, {
        method: "POST",
      });
      if (res.ok) {
        await fetchOrders();
        await fetchStock();
      } else {
        alert("Failed to complete order");
      }
    } catch {
      alert("Erro ao completar pedido");
    }
  };

  const styles = {
    container: {
      maxWidth: 700,
      margin: "40px auto",
      padding: "20px",
      fontFamily: "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif",
      color: "#333",
      minHeight: "100vh",
    },
    header: {
      textAlign: "center",
      marginBottom: 20,
      color: "#2c3e50",
    },
    stock: {
      fontWeight: "bold",
      fontSize: 18,
      marginBottom: 15,
    },
    inputContainer: {
      display: "flex",
      alignItems: "center",
      gap: 10,
      marginBottom: 15,
    },
    input: {
      width: 70,
      padding: "6px 8px",
      fontSize: 16,
      borderRadius: 4,
      border: "1px solid #ccc",
    },
    button: {
      padding: "8px 16px",
      fontSize: 16,
      borderRadius: 4,
      backgroundColor: "#3498db",
      color: "white",
      border: "none",
      cursor: "pointer",
      transition: "background-color 0.3s",
    },
    buttonHover: {
      backgroundColor: "#2980b9",
    },
    error: {
      color: "crimson",
      marginBottom: 20,
      fontWeight: "600",
    },
    table: {
      width: "100%",
      borderCollapse: "collapse",
    },
    th: {
      borderBottom: "2px solid #ddd",
      padding: "10px 8px",
      textAlign: "left",
      backgroundColor: "#f8f8f8",
      color: "#34495e",
    },
    td: {
      borderBottom: "1px solid #eee",
      padding: "8px",
    },
    completeButton: {
      padding: "6px 12px",
      fontSize: 14,
      backgroundColor: "#27ae60",
      color: "white",
      border: "none",
      borderRadius: 4,
      cursor: "pointer",
      transition: "background-color 0.3s",
    },
    completeButtonHover: {
      backgroundColor: "#1e8449",
    },
    noOrdersRow: {
      textAlign: "center",
      color: "#999",
      fontStyle: "italic",
      padding: 20,
    },
    errorTip: {
      backgroundColor: "#fdecea",
      color: "#d32f2f",
      border: "1px solid #f5c6cb",
      borderRadius: 5,
      padding: "10px 12px",
      marginBottom: 20,
      display: "flex",
      alignItems: "center",
      fontWeight: 500,
      fontSize: 14,
    },
    errorIcon: {
      marginRight: 8,
      fontSize: 18,
    },
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.header}>Order Manager</h2>

      <div style={styles.stock}>Available Stock: {stock}</div>
      <div style={styles.stock}>Reserved: {reserved}</div>

      <div style={styles.inputContainer}>
        <input
          type="number"
          min={1}
          value={quantity}
          onChange={(e) => setQuantity(Number(e.target.value))}
          style={styles.input}
        />
        <button
          onClick={handleCreateOrder}
          style={styles.button}
          onMouseOver={(e) => (e.currentTarget.style.backgroundColor = styles.buttonHover.backgroundColor)}
          onMouseOut={(e) => (e.currentTarget.style.backgroundColor = styles.button.backgroundColor)}
        >
          Create Order
        </button>
      </div>

      {error && (
        <div style={styles.errorTip}>
          <span style={styles.errorIcon}>❌</span> {error}
        </div>
      )}
      <h3>Orders</h3>
      <table style={styles.table}>
        <thead>
          <tr>
            <th style={styles.th}>ID</th>
            <th style={styles.th}>Quantity</th>
            <th style={styles.th}>Completed</th>
            <th style={styles.th}>Created At</th>
            <th style={styles.th}>Action</th>
          </tr>
        </thead>
        <tbody>
          {orders.length === 0 && (
            <tr>
              <td colSpan={5} style={styles.noOrdersRow}>
                No orders found
              </td>
            </tr>
          )}
          {orders.map((order) => (
            <tr key={order.id}>
              <td style={styles.td}>{order.id}</td>
              <td style={styles.td}>{order.quantity}</td>
              <td style={styles.td}>{order.completed ? "Yes" : "No"}</td>
              <td style={styles.td}>{new Date(order.createdAt).toLocaleString()}</td>
              <td style={styles.td}>
                {!order.completed && (
                  <button
                    onClick={() => handleCompleteOrder(order.id)}
                    style={styles.completeButton}
                    onMouseOver={(e) =>
                      (e.currentTarget.style.backgroundColor = styles.completeButtonHover.backgroundColor)
                    }
                    onMouseOut={(e) =>
                      (e.currentTarget.style.backgroundColor = styles.completeButton.backgroundColor)
                    }
                  >
                    Complete
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

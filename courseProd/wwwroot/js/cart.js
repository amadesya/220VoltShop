window.courseProdCart = {
  get: function() {
    try {
      const raw = localStorage.getItem('courseProdCart');
      return raw ? JSON.parse(raw) : [];
    } catch(e) { return []; }
  },
  set: function(items) {
    try { localStorage.setItem('courseProdCart', JSON.stringify(items)); return true; } catch(e) { return false; }
  },
  add: function(item) {
    try {
      const items = window.courseProdCart.get();
      // if exists by sku or id, increase qty
      const existing = items.find(x => x.sku === item.sku || x.id === item.id);
      if (existing) { existing.quantity = (existing.quantity || 1) + (item.quantity || 1); }
      else { item.quantity = item.quantity || 1; items.push(item); }
      localStorage.setItem('courseProdCart', JSON.stringify(items));
      return items;
    } catch(e) { return null; }
  },
    updateCartQuantity: function(sku, qty) {
      const items = window.courseProdCart.get();
      const idx = items.findIndex(x => x.sku === sku);
      if (idx >= 0) {
        if (qty <= 0) {
          items.splice(idx, 1);
        } else {
          items[idx].quantity = qty;
        }
        window.courseProdCart.set(items);
      }
      return items;
    },
    removeFromCart: function(sku) {
      const items = window.courseProdCart.get();
      const idx = items.findIndex(x => x.sku === sku);
      if (idx >= 0) {
        items.splice(idx, 1);
        window.courseProdCart.set(items);
      }
      return items;
    },
  clear: function() { localStorage.removeItem('courseProdCart'); return []; }
};

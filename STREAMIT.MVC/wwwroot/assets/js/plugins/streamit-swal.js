/**
 * streamit-swal.js
 * Global SweetAlert2 helpers for STREAMIT.
 * Loaded in _Layout.cshtml after sweetalert2.min.js.
 *
 * Exports on window.Swal (already global) + window.StreamSwal helpers.
 */

(function () {
  'use strict';

  /* ── Base theme shared by all dialogs ───────────────────────── */
  const BASE = {
    background: '#1a1a1a',
    color: '#e8e8e8',
    confirmButtonColor: '#e50914',
    cancelButtonColor: '#2a2a2a',
    backdrop: 'rgba(0,0,0,0.75)',
  };

  /* ── Toast (top-end, auto-close) ────────────────────────────── */
  const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3200,
    timerProgressBar: true,
    background: '#1a1a1a',
    color: '#e8e8e8',
    didOpen: (toast) => {
      toast.addEventListener('mouseenter', Swal.stopTimer);
      toast.addEventListener('mouseleave', Swal.resumeTimer);
    },
  });

  /* ── Public API ──────────────────────────────────────────────── */
  window.StreamSwal = {

    /** ✅ Success toast */
    success(msg, title) {
      Toast.fire({ icon: 'success', title: title || msg, text: title ? msg : undefined });
    },

    /** ❌ Error toast */
    error(msg, title) {
      Toast.fire({ icon: 'error', title: title || msg, text: title ? msg : undefined });
    },

    /** ℹ️ Info toast */
    info(msg) {
      Toast.fire({ icon: 'info', title: msg });
    },

    /** ⚠️ Warning toast */
    warning(msg) {
      Toast.fire({ icon: 'warning', title: msg });
    },

    /**
     * 🗑️ Delete / danger confirm dialog.
     * Returns a Promise<boolean> — true if confirmed.
     *
     * Usage:
     *   if (await StreamSwal.confirmDelete('Remove from watchlist?')) { ... }
     */
    async confirmDelete(text, title) {
      const result = await Swal.fire({
        ...BASE,
        icon: 'warning',
        title: title || 'Are you sure?',
        text: text || 'This action cannot be undone.',
        showCancelButton: true,
        confirmButtonText: '<i class="ph ph-trash me-1"></i> Yes, remove it',
        cancelButtonText: 'Cancel',
        reverseButtons: true,
        focusCancel: true,
      });
      return result.isConfirmed;
    },

    /**
     * 🔒 Logout confirm dialog.
     * Returns Promise<boolean>.
     */
    async confirmLogout() {
      const result = await Swal.fire({
        ...BASE,
        icon: 'question',
        title: 'Logging out?',
        text: 'You will be returned to the login screen.',
        showCancelButton: true,
        confirmButtonText: '<i class="ph ph-sign-out me-1"></i> Yes, log out',
        cancelButtonText: 'Stay',
        reverseButtons: true,
        focusCancel: true,
      });
      return result.isConfirmed;
    },

    /**
     * Generic confirm dialog.
     * Returns Promise<boolean>.
     */
    async confirm(title, text, confirmLabel) {
      const result = await Swal.fire({
        ...BASE,
        icon: 'question',
        title,
        text,
        showCancelButton: true,
        confirmButtonText: confirmLabel || 'Confirm',
        cancelButtonText: 'Cancel',
        reverseButtons: true,
      });
      return result.isConfirmed;
    },
  };

  /* ── Replace native browser alert() globally ─────────────────
     Any leftover alert('...') call in the app will now pop up
     as a styled Swal instead of the ugly browser native dialog.
  ──────────────────────────────────────────────────────────────── */
  window._nativeAlert = window.alert;
  window.alert = function (msg) {
    Swal.fire({ ...BASE, icon: 'info', title: String(msg) });
  };

  /* ── TempData toasts ─────────────────────────────────────────
     _Layout renders hidden spans with ids #swal-success / #swal-error.
     We read them on DOMContentLoaded and fire toasts automatically.
  ──────────────────────────────────────────────────────────────── */
  document.addEventListener('DOMContentLoaded', function () {

    const successEl = document.getElementById('swal-success-msg');
    const errorEl   = document.getElementById('swal-error-msg');

    if (successEl && successEl.dataset.msg) {
      StreamSwal.success(successEl.dataset.msg);
    }
    if (errorEl && errorEl.dataset.msg) {
      StreamSwal.error(errorEl.dataset.msg);
    }

    /* ── Logout confirm ──────────────────────────────────────── */
    document.querySelectorAll('[data-swal-logout]').forEach(function (btn) {
      btn.addEventListener('click', async function (e) {
        e.preventDefault();
        if (await StreamSwal.confirmLogout()) {
          // Submit the closest form (the logout <form> in _Layout)
          const form = btn.closest('form');
          if (form) form.submit();
        }
      });
    });

    /* ── Generic delete confirms ─────────────────────────────── */
    // Any element with data-swal-delete="Message text" will get a confirm
    document.querySelectorAll('[data-swal-delete]').forEach(function (el) {
      el.addEventListener('click', async function (e) {
        e.preventDefault();
        const confirmed = await StreamSwal.confirmDelete(el.dataset.swalDelete);
        if (confirmed) {
          const form = el.closest('form');
          if (form) { form.submit(); return; }
          const href = el.getAttribute('href');
          if (href && href !== '#') window.location.href = href;
        }
      });
    });
  });

})();

class Session {

	constructor() {
		this.session = Cookies.get('sessionId');
	}

	clearSession() {
	    Cookies.remove('sessionId');
	    this.session = undefined;
	}

	createSession(username, password, accountId) {
		if (this.session != undefined) {
			alert("Please log out first!");
			return;
		}

		$.ajax({
        url: '/apiv2/auth/createSession',
        type: 'POST',
        data: {
          username: username,
          password: password,
          account: accountId
        },
        success: function(data) {
        	Cookies.set('sessionId', data);
          document.location = '../';
        },
        error: function(xhr, status, error) {
        	alert(error);
        }
      });
	}
}
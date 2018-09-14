class Session {

	constructor() {
		this.session = Cookies.get('sessionId');
    this.username = Cookies.get('loggedin_name');
	}

	clearSession() {
	    Cookies.remove('sessionId');
      Cookies.remove('loggedin_name');
	    this.session = undefined;
      this.username = undefined;
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
          $.ajax({
            url: '/apiv2/users/getUserForSession',
            type: 'POST',
            async: false,
            data: {
              session: data
            },
            success: function(data) {
              Cookies.set('loggedin_name', data.Name);
            }
          });
          document.location = '../';
        },
        error: function(xhr, status, error) {
        	alert(error);
        }
      });
	}
}